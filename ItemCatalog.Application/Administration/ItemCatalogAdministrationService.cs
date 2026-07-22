using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Application.Items;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.ItemCatalog.Application.Administration;

public sealed class ItemCatalogAdministrationService
{
    private readonly IItemDefinitionRepository _itemDefinitionRepository;
    private readonly IItemCatalogAdministrativeAccess _administrativeAccess;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public ItemCatalogAdministrationService(
        IItemDefinitionRepository itemDefinitionRepository,
        IItemCatalogAdministrativeAccess administrativeAccess,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider )
    {
        _itemDefinitionRepository = itemDefinitionRepository;
        _administrativeAccess = administrativeAccess;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<ItemRevisionDto> CreateDraftAsync(
        CreateItemDraftRequest request,
        CancellationToken cancellationToken )
    {
        await EnsureAccessAsync(
            request.Scope,
            request.CampaignId,
            request.ActingUserId,
            request.ActingUserName,
            cancellationToken );
        string normalizedKey = request.Key?.Trim() ?? String.Empty;
        ItemDefinition? definition = await FindDefinitionAsync(
            request.Scope,
            request.CampaignId,
            normalizedKey,
            cancellationToken );
        if ( definition is null )
        {
            definition = request.Scope == ItemCatalogScope.Global
                ? ItemDefinition.CreateGlobal( normalizedKey, _timeProvider.GetUtcNow() )
                : ItemDefinition.CreateForCampaign(
                    normalizedKey,
                    request.CampaignId!.Value,
                    _timeProvider.GetUtcNow() );
            _itemDefinitionRepository.Add( definition );
        }

        ItemRevision revision = definition.CreateRevision(
            request.Name,
            request.Description,
            request.Level,
            request.PriceInCopperPieces,
            request.Bulk,
            request.Rules,
            _timeProvider.GetUtcNow() );
        await _unitOfWork.Commit();
        return ToDto( definition, revision );
    }

    public async Task<ItemRevisionDto> PublishAsync(
        int itemDefinitionId,
        int revisionNumber,
        int actingUserId,
        string actingUserName,
        CancellationToken cancellationToken )
    {
        ItemDefinition definition = await GetAuthorizedDefinitionAsync(
            itemDefinitionId,
            actingUserId,
            actingUserName,
            cancellationToken );
        ItemRevision revision = definition.PublishRevision(
            revisionNumber,
            _timeProvider.GetUtcNow() );
        await _unitOfWork.Commit();
        return ToDto( definition, revision );
    }

    public async Task<ItemRevisionDto> RetireAsync(
        int itemDefinitionId,
        int revisionNumber,
        int actingUserId,
        string actingUserName,
        CancellationToken cancellationToken )
    {
        ItemDefinition definition = await GetAuthorizedDefinitionAsync(
            itemDefinitionId,
            actingUserId,
            actingUserName,
            cancellationToken );
        ItemRevision revision = definition.RetireRevision(
            revisionNumber,
            _timeProvider.GetUtcNow() );
        await _unitOfWork.Commit();
        return ToDto( definition, revision );
    }

    private async Task<ItemDefinition> GetAuthorizedDefinitionAsync(
        int itemDefinitionId,
        int actingUserId,
        string actingUserName,
        CancellationToken cancellationToken )
    {
        ItemDefinition? definition = await _itemDefinitionRepository.GetByIdWithRevisionsAsync(
            itemDefinitionId,
            cancellationToken );
        if ( definition is null )
        {
            throw new ItemCatalogApplicationException( "Item definition was not found." );
        }

        await EnsureAccessAsync(
            definition.Scope,
            definition.CampaignId,
            actingUserId,
            actingUserName,
            cancellationToken );
        return definition;
    }

    private async Task<ItemDefinition?> FindDefinitionAsync(
        ItemCatalogScope scope,
        int? campaignId,
        string key,
        CancellationToken cancellationToken )
    {
        return scope == ItemCatalogScope.Global
            ? await _itemDefinitionRepository.GetGlobalByKeyWithRevisionsAsync(
                key,
                cancellationToken )
            : await _itemDefinitionRepository.GetCampaignByKeyWithRevisionsAsync(
                key,
                campaignId!.Value,
                cancellationToken );
    }

    private async Task EnsureAccessAsync(
        ItemCatalogScope scope,
        int? campaignId,
        int actingUserId,
        string actingUserName,
        CancellationToken cancellationToken )
    {
        bool hasAccess;
        if ( scope == ItemCatalogScope.Global )
        {
            if ( campaignId is not null )
            {
                throw new ItemCatalogApplicationException(
                    "Global item definitions cannot have a campaign id." );
            }

            hasAccess = await _administrativeAccess.CanManageGlobalCatalogAsync(
                actingUserName,
                cancellationToken );
        }
        else if ( scope == ItemCatalogScope.Campaign )
        {
            if ( campaignId is null or <= 0 )
            {
                throw new ItemCatalogApplicationException(
                    "Campaign item definitions require a positive campaign id." );
            }

            hasAccess = await _administrativeAccess.CanManageCampaignCatalogAsync(
                actingUserId,
                campaignId.Value,
                cancellationToken );
        }
        else
        {
            throw new ItemCatalogApplicationException( "Item catalog scope is invalid." );
        }

        if ( !hasAccess )
        {
            throw new ItemCatalogAccessDeniedException(
                "Current user cannot manage this item catalog scope." );
        }
    }

    private static ItemRevisionDto ToDto( ItemDefinition definition, ItemRevision revision ) =>
        new ItemRevisionDto(
            definition.Id,
            definition.Key,
            definition.Scope,
            definition.CampaignId,
            revision.RevisionNumber,
            revision.Name,
            revision.Description,
            revision.Level,
            revision.PriceInCopperPieces,
            revision.Bulk,
            revision.PrimaryCategory,
            revision.Status,
            revision.CreatedAtUtc,
            revision.PublishedAtUtc,
            revision.RetiredAtUtc );
}
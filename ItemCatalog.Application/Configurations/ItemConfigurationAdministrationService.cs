using Pathfinder.ItemCatalog.Application.Administration;
using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Application.Items;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.ItemCatalog.Application.Configurations;

public sealed class ItemConfigurationAdministrationService
{
    private readonly IItemDefinitionRepository _itemDefinitionRepository;
    private readonly IItemConfigurationRepository _itemConfigurationRepository;
    private readonly IItemCatalogAdministrativeAccess _administrativeAccess;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public ItemConfigurationAdministrationService(
        IItemDefinitionRepository itemDefinitionRepository,
        IItemConfigurationRepository itemConfigurationRepository,
        IItemCatalogAdministrativeAccess administrativeAccess,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider )
    {
        _itemDefinitionRepository = itemDefinitionRepository;
        _itemConfigurationRepository = itemConfigurationRepository;
        _administrativeAccess = administrativeAccess;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<ItemConfigurationDto> CreateAsync(
        CreateItemConfigurationRequest request,
        CancellationToken cancellationToken )
    {
        bool canManage = await _administrativeAccess.CanManageCampaignCatalogAsync(
            request.ActingUserId,
            request.CampaignId,
            cancellationToken );
        if ( !canManage )
        {
            throw new ItemCatalogAccessDeniedException(
                "Current user cannot manage item configurations for this campaign." );
        }

        ItemDefinition definition = await _itemDefinitionRepository.GetByIdWithRevisionsAsync(
            request.ItemDefinitionId,
            cancellationToken ) ?? throw new ItemCatalogApplicationException(
            "Item definition was not found in this campaign." );
        if ( definition.Scope == ItemCatalogScope.Campaign &&
             definition.CampaignId != request.CampaignId )
        {
            throw new ItemCatalogApplicationException(
                "Item definition was not found in this campaign." );
        }

        ItemRevision revision = definition.Revisions
            .SingleOrDefault( item => item.RevisionNumber == request.RevisionNumber )
            ?? throw new ItemCatalogApplicationException( "Item revision was not found." );
        if ( revision.Status == ItemRevisionStatus.Draft )
        {
            if ( definition.Scope == ItemCatalogScope.Global )
            {
                throw new ItemCatalogApplicationException( "Item revision was not found." );
            }

            throw new ItemCatalogApplicationException(
                "Item configuration requires a published revision, but the revision is still a draft." );
        }

        if ( revision.Status == ItemRevisionStatus.Retired )
        {
            throw new ItemCatalogApplicationException(
                "Item configuration requires a published revision, but the revision has been retired." );
        }

        ItemConfiguration candidate = ItemConfiguration.Create(
            request.CampaignId,
            revision.Id,
            request.Size,
            request.MaterialType,
            request.MaterialGrade,
            request.PermanentUpgrades,
            _timeProvider.GetUtcNow() );
        ItemConfiguration? existing = await _itemConfigurationRepository.GetByConfigurationKeyAsync(
            candidate.ConfigurationKey,
            cancellationToken );
        if ( existing is not null )
        {
            return ToDto( existing, wasCreated: false );
        }

        _itemConfigurationRepository.Add( candidate );
        try
        {
            await _unitOfWork.Commit();
        }
        catch ( Exception )
        {
            ItemConfiguration? concurrent = await _itemConfigurationRepository.GetByConfigurationKeyAsync(
                candidate.ConfigurationKey,
                cancellationToken );
            if ( concurrent is not null )
            {
                return ToDto( concurrent, wasCreated: false );
            }

            throw;
        }

        return ToDto( candidate, wasCreated: true );
    }

    private static ItemConfigurationDto ToDto( ItemConfiguration configuration, bool wasCreated ) =>
        new ItemConfigurationDto(
            configuration.Id,
            configuration.CampaignId,
            configuration.ItemRevisionId,
            configuration.ConfigurationKey,
            configuration.Size,
            configuration.MaterialType,
            configuration.MaterialGrade,
            configuration.PermanentUpgrades
                .Select( upgrade => new PermanentUpgradeDto(
                    upgrade.Code,
                    upgrade.Kind,
                    upgrade.Rank,
                    upgrade.Visibility ) )
                .ToArray(),
            wasCreated );
}
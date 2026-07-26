using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Knowledge;
using Pathfinder.ItemCatalog.Infrastructure.Data;

namespace Pathfinder.Web.Integration;

public sealed class ItemKnowledgeAdministrationService
{
    private readonly ItemObservationService _observationService;
    private readonly IItemObservationAccess _observationAccess;
    private readonly ItemCatalogDbContext _itemCatalogDbContext;
    private readonly CampaignManagementDbContext _campaignDbContext;
    private readonly TimeProvider _timeProvider;

    public ItemKnowledgeAdministrationService(
        ItemObservationService observationService,
        IItemObservationAccess observationAccess,
        ItemCatalogDbContext itemCatalogDbContext,
        CampaignManagementDbContext campaignDbContext,
        TimeProvider timeProvider )
    {
        _observationService = observationService;
        _observationAccess = observationAccess;
        _itemCatalogDbContext = itemCatalogDbContext;
        _campaignDbContext = campaignDbContext;
        _timeProvider = timeProvider;
    }

    public async Task<ItemPropertyKnowledgeDto> RevealAsync(
        RevealItemPropertyRequest request,
        CancellationToken cancellationToken )
    {
        ItemObservationAccess access = await _observationAccess.GetAccessAsync(
            request.CampaignId,
            request.ActingUserId,
            cancellationToken );
        if ( !access.IsGameMaster )
        {
            throw new ItemCatalogAccessDeniedException(
                "Only a game master can reveal hidden item properties." );
        }

        ResolvedItemDto item = await _observationService.ResolveAsync(
            request.CampaignId,
            request.InstanceKey,
            cancellationToken );
        bool isHiddenProperty = item.PermanentUpgrades.Any( upgrade =>
            upgrade.Visibility == PermanentUpgradeVisibility.Hidden &&
            String.Equals(
                upgrade.Code,
                request.UpgradeCode,
                StringComparison.Ordinal ) );
        if ( !isHiddenProperty )
        {
            throw new InvalidOperationException(
                "Hidden item property was not found." );
        }

        await EnsureSubjectBelongsToCampaignAsync(
            request.CampaignId,
            request.SubjectKind,
            request.SubjectId,
            cancellationToken );
        string normalizedCode = request.UpgradeCode.Trim();
        ItemPropertyKnowledge? knowledge =
            await _itemCatalogDbContext.ItemPropertyKnowledgeEntries.SingleOrDefaultAsync(
                itemKnowledge =>
                    itemKnowledge.CampaignId == request.CampaignId &&
                    itemKnowledge.InstanceKey == request.InstanceKey &&
                    itemKnowledge.SubjectKind == request.SubjectKind &&
                    itemKnowledge.SubjectId == request.SubjectId &&
                    itemKnowledge.UpgradeCode == normalizedCode,
                cancellationToken );
        if ( knowledge is null )
        {
            knowledge = ItemPropertyKnowledge.Create(
                request.CampaignId,
                request.InstanceKey,
                request.SubjectKind,
                request.SubjectId,
                normalizedCode,
                request.ActingUserId,
                _timeProvider.GetUtcNow() );
            _itemCatalogDbContext.ItemPropertyKnowledgeEntries.Add( knowledge );
            await _itemCatalogDbContext.SaveChangesAsync( cancellationToken );
        }

        return new ItemPropertyKnowledgeDto(
            knowledge.InstanceKey,
            knowledge.SubjectKind,
            knowledge.SubjectId,
            knowledge.UpgradeCode,
            knowledge.RevealedAtUtc );
    }

    private async Task EnsureSubjectBelongsToCampaignAsync(
        int campaignId,
        ItemKnowledgeSubjectKind subjectKind,
        int subjectId,
        CancellationToken cancellationToken )
    {
        bool exists = subjectKind switch
        {
            ItemKnowledgeSubjectKind.Character =>
                await _campaignDbContext.CampaignPartyCharacters
                    .Join(
                        _campaignDbContext.CampaignParties,
                        assignment => assignment.CampaignPartyId,
                        party => party.Id,
                        ( assignment, party ) => new
                        {
                            assignment.CharacterId,
                            party.CampaignId,
                            party.Status,
                        } )
                    .AnyAsync(
                        item =>
                            item.CampaignId == campaignId &&
                            item.Status == CampaignPartyStatus.Active &&
                            item.CharacterId == subjectId,
                        cancellationToken ),
            ItemKnowledgeSubjectKind.Party =>
                await _campaignDbContext.CampaignParties.AnyAsync(
                    party =>
                        party.CampaignId == campaignId &&
                        party.Id == subjectId &&
                        party.Status == CampaignPartyStatus.Active,
                    cancellationToken ),
            _ => false,
        };
        if ( !exists )
        {
            throw new InvalidOperationException(
                "Knowledge subject was not found in this campaign." );
        }
    }
}

public sealed record RevealItemPropertyRequest(
    int CampaignId,
    Guid InstanceKey,
    ItemKnowledgeSubjectKind SubjectKind,
    int SubjectId,
    string UpgradeCode,
    int ActingUserId );

public sealed record ItemPropertyKnowledgeDto(
    Guid InstanceKey,
    ItemKnowledgeSubjectKind SubjectKind,
    int SubjectId,
    string UpgradeCode,
    DateTimeOffset RevealedAtUtc );
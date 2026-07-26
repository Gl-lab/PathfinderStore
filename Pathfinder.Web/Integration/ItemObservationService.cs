using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Infrastructure.Data;

namespace Pathfinder.Web.Integration;

public sealed class ItemObservationService
{
    private readonly InventoryDbContext _inventoryDbContext;
    private readonly ItemCatalogDbContext _itemCatalogDbContext;
    private readonly IItemObservationAccess _observationAccess;

    public ItemObservationService(
        InventoryDbContext inventoryDbContext,
        ItemCatalogDbContext itemCatalogDbContext,
        IItemObservationAccess observationAccess )
    {
        _inventoryDbContext = inventoryDbContext;
        _itemCatalogDbContext = itemCatalogDbContext;
        _observationAccess = observationAccess;
    }

    public async Task<ResolvedItemDto> ResolveAsync(
        int campaignId,
        Guid instanceKey,
        CancellationToken cancellationToken )
    {
        ItemInstance instance = await _inventoryDbContext.ItemInstances
            .SingleOrDefaultAsync(
                item =>
                    item.CampaignId == campaignId &&
                    item.InstanceKey == instanceKey,
                cancellationToken )
            ?? throw new InventoryException( "Item instance was not found in this campaign." );
        ItemConfiguration configuration = await _itemCatalogDbContext.ItemConfigurations
            .Include( item => item.PermanentUpgrades )
            .SingleOrDefaultAsync(
                item =>
                    item.Id == instance.ItemConfigurationId &&
                    (item.CampaignId == campaignId || item.CampaignId == null),
                cancellationToken )
            ?? throw new InventoryException(
                "Item configuration was not found for this campaign." );
        ItemRevision revision = await _itemCatalogDbContext.ItemRevisions
            .SingleAsync(
                item => item.Id == configuration.ItemRevisionId,
                cancellationToken );
        ItemDefinition definition = await _itemCatalogDbContext.ItemDefinitions
            .SingleAsync(
                item => item.Id == revision.ItemDefinitionId,
                cancellationToken );
        ResolvedUpgradeDto[] upgrades = configuration.PermanentUpgrades
            .Select( ToDto )
            .ToArray();
        return new ResolvedItemDto(
            instance.InstanceKey,
            campaignId,
            instance.CustomName ?? revision.Name,
            definition.Key,
            revision.RevisionNumber,
            revision.Description,
            revision.Level,
            revision.PriceInCopperPieces,
            revision.Bulk,
            revision.PrimaryCategory,
            upgrades );
    }

    public async Task<VisibleItemDto> GetVisibleAsync(
        int campaignId,
        Guid instanceKey,
        int observerUserId,
        CancellationToken cancellationToken )
    {
        ItemObservationAccess access = await _observationAccess.GetAccessAsync(
            campaignId,
            observerUserId,
            cancellationToken );
        if ( !access.IsMember )
        {
            throw new ItemObservationAccessDeniedException();
        }

        ResolvedItemDto resolved = await ResolveAsync(
            campaignId,
            instanceKey,
            cancellationToken );
        IReadOnlyCollection<ResolvedUpgradeDto> upgrades = access.IsGameMaster
            ? resolved.PermanentUpgrades
            : resolved.PermanentUpgrades
                .Where( item => item.Visibility == PermanentUpgradeVisibility.Public )
                .ToArray();
        return new VisibleItemDto(
            resolved.InstanceKey,
            resolved.CampaignId,
            resolved.Name,
            resolved.DefinitionKey,
            resolved.RevisionNumber,
            resolved.Description,
            resolved.Level,
            resolved.PriceInCopperPieces,
            resolved.Bulk,
            resolved.PrimaryCategory,
            upgrades,
            access.IsGameMaster );
    }

    private static ResolvedUpgradeDto ToDto( PermanentUpgrade upgrade ) =>
        new ResolvedUpgradeDto(
            upgrade.Code,
            upgrade.Kind,
            upgrade.Rank,
            upgrade.Visibility );
}

public interface IItemObservationAccess
{
    Task<ItemObservationAccess> GetAccessAsync(
        int campaignId,
        int observerUserId,
        CancellationToken cancellationToken );
}

public sealed record ItemObservationAccess( bool IsMember, bool IsGameMaster );

public sealed class ItemObservationAccessDeniedException : Exception
{
    public ItemObservationAccessDeniedException()
        : base( "Current user cannot observe items in this campaign." )
    {
    }
}

public sealed record ResolvedItemDto(
    Guid InstanceKey,
    int CampaignId,
    string Name,
    string DefinitionKey,
    int RevisionNumber,
    string Description,
    int Level,
    int PriceInCopperPieces,
    decimal Bulk,
    Pathfinder.ItemCatalog.Domain.Rules.ItemCategory PrimaryCategory,
    IReadOnlyCollection<ResolvedUpgradeDto> PermanentUpgrades );

public sealed record ResolvedUpgradeDto(
    string Code,
    PermanentUpgradeKind Kind,
    int Rank,
    PermanentUpgradeVisibility Visibility );

public sealed record VisibleItemDto(
    Guid InstanceKey,
    int CampaignId,
    string Name,
    string DefinitionKey,
    int RevisionNumber,
    string Description,
    int Level,
    int PriceInCopperPieces,
    decimal Bulk,
    Pathfinder.ItemCatalog.Domain.Rules.ItemCategory PrimaryCategory,
    IReadOnlyCollection<ResolvedUpgradeDto> PermanentUpgrades,
    bool IncludesHiddenProperties );
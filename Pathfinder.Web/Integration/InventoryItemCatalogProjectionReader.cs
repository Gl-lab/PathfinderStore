using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;

namespace Pathfinder.Web.Integration;

public sealed class InventoryItemCatalogProjectionReader
{
    private readonly ItemCatalogDbContext _dbContext;

    public InventoryItemCatalogProjectionReader( ItemCatalogDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<int, InventoryItemCatalogProjection>> ReadAsync(
        int campaignId,
        IReadOnlyCollection<ItemInstance> instances,
        CancellationToken cancellationToken )
    {
        int[] configurationIds = instances
            .Select( instance => instance.ItemConfigurationId )
            .Distinct()
            .ToArray();
        return await ReadAsync(
            campaignId,
            configurationIds,
            cancellationToken );
    }

    public async Task<Dictionary<int, InventoryItemCatalogProjection>> ReadAsync(
        int campaignId,
        IReadOnlyCollection<int> itemConfigurationIds,
        CancellationToken cancellationToken )
    {
        int[] configurationIds = itemConfigurationIds
            .Distinct()
            .ToArray();
        if ( configurationIds.Length == 0 )
        {
            return [];
        }

        ItemConfiguration[] configurations = await _dbContext.ItemConfigurations
            .AsNoTracking()
            .Where( configuration =>
                configurationIds.Contains( configuration.Id ) &&
                ( configuration.CampaignId == null ||
                  configuration.CampaignId == campaignId ) )
            .ToArrayAsync( cancellationToken );
        if ( configurations.Length != configurationIds.Length )
        {
            throw new InvalidOperationException(
                "Inventory contains a missing or cross-campaign item configuration." );
        }

        int[] revisionIds = configurations
            .Select( configuration => configuration.ItemRevisionId )
            .Distinct()
            .ToArray();
        ItemRevision[] revisions = await _dbContext.ItemRevisions
            .AsNoTracking()
            .Where( revision => revisionIds.Contains( revision.Id ) )
            .ToArrayAsync( cancellationToken );
        if ( revisions.Length != revisionIds.Length )
        {
            throw new InvalidOperationException( "Inventory contains a missing item revision." );
        }

        int[] definitionIds = revisions
            .Select( revision => revision.ItemDefinitionId )
            .Distinct()
            .ToArray();
        ItemDefinition[] definitions = await _dbContext.ItemDefinitions
            .AsNoTracking()
            .Where( definition => definitionIds.Contains( definition.Id ) )
            .ToArrayAsync( cancellationToken );
        bool definitionsAreVisible =
            definitions.Length == definitionIds.Length &&
            definitions.All( definition =>
                definition.Scope == ItemCatalogScope.Global ||
                ( definition.Scope == ItemCatalogScope.Campaign &&
                  definition.CampaignId == campaignId ) );
        if ( !definitionsAreVisible )
        {
            throw new InvalidOperationException(
                "Inventory contains a missing or cross-campaign item definition." );
        }

        Dictionary<int, ItemRevision> revisionsById = revisions
            .ToDictionary( revision => revision.Id );
        return configurations.ToDictionary(
            configuration => configuration.Id,
            configuration =>
            {
                ItemRevision revision = revisionsById[ configuration.ItemRevisionId ];
                return new InventoryItemCatalogProjection(
                    revision.RevisionNumber,
                    revision.Name,
                    revision.Description,
                    revision.Level,
                    revision.PrimaryCategory,
                    revision.PriceInCopperPieces,
                    ToBulkTenths( revision.Bulk ) );
            } );
    }

    private static int ToBulkTenths( decimal bulk )
    {
        decimal tenths = bulk * 10;
        if ( tenths != Decimal.Truncate( tenths ) )
        {
            throw new InvalidOperationException(
                "Published item Bulk must use tenths for inventory projection." );
        }

        return Decimal.ToInt32( tenths );
    }
}

public sealed record InventoryItemCatalogProjection(
    int RevisionNumber,
    string Name,
    string Description,
    int Level,
    ItemCategory PrimaryCategory,
    int PriceInCopperPieces,
    int BulkTenths );

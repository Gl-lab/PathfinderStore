using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Equipment;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Equipment;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Infrastructure.Data;

namespace Pathfinder.Web.Integration;

public sealed class RuntimeInventoryAllowedEquipmentReader : IAllowedEquipmentReader
{
    private readonly InventoryDbContext _inventoryDbContext;
    private readonly ItemCatalogDbContext _itemCatalogDbContext;
    private readonly ItemCatalogAllowedEquipmentReader _fallbackReader;

    public RuntimeInventoryAllowedEquipmentReader(
        InventoryDbContext inventoryDbContext,
        ItemCatalogDbContext itemCatalogDbContext,
        ItemCatalogAllowedEquipmentReader fallbackReader )
    {
        _inventoryDbContext = inventoryDbContext;
        _itemCatalogDbContext = itemCatalogDbContext;
        _fallbackReader = fallbackReader;
    }

    public AllowedEquipmentLoadout Read(
        DraftCharacter character,
        IReadOnlyList<EffectiveProficiency> proficiencies,
        int? campaignId = null )
    {
        ArgumentNullException.ThrowIfNull( character );
        ArgumentNullException.ThrowIfNull( proficiencies );
        if ( !character.HasRuntimeInventory )
        {
            return _fallbackReader.Read( character, proficiencies, campaignId );
        }

        if ( campaignId is <= 0 )
        {
            throw new ArgumentOutOfRangeException( nameof( campaignId ) );
        }

        Guid[] instanceKeys = character.RuntimeEquipmentItems
            .Select( item => item.ItemInstanceKey )
            .ToArray();
        ItemInstance[] instances = _inventoryDbContext.ItemInstances
            .AsNoTracking()
            .Where( instance => instanceKeys.Contains( instance.InstanceKey ) )
            .ToArray();
        if ( instances.Length != instanceKeys.Length )
        {
            throw new InvalidOperationException(
                "Character runtime inventory contains missing item instances." );
        }

        ValidateOwnership( character, instances, campaignId );
        int[] configurationIds = instances
            .Select( instance => instance.ItemConfigurationId )
            .Distinct()
            .ToArray();
        ItemConfiguration[] configurations = _itemCatalogDbContext.ItemConfigurations
            .AsNoTracking()
            .Where( configuration => configurationIds.Contains( configuration.Id ) )
            .ToArray();
        if ( configurations.Length != configurationIds.Length )
        {
            throw new InvalidOperationException(
                "Character runtime inventory contains missing item configurations." );
        }

        int[] revisionIds = configurations
            .Select( configuration => configuration.ItemRevisionId )
            .Distinct()
            .ToArray();
        ItemDefinition[] definitions = _itemCatalogDbContext.ItemDefinitions
            .AsNoTracking()
            .AsSplitQuery()
            .Where( definition => definition.Revisions.Any(
                revision => revisionIds.Contains( revision.Id ) ) )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Attacks )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Armor )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Shield )
            .Include( definition => definition.Revisions )
                .ThenInclude( revision => revision.Durability )
            .ToArray();
        Dictionary<int, ItemRevision> revisionsById = definitions
            .SelectMany( definition => definition.Revisions )
            .Where( revision => revisionIds.Contains( revision.Id ) )
            .ToDictionary( revision => revision.Id );
        if ( revisionsById.Count != revisionIds.Length )
        {
            throw new InvalidOperationException(
                "Character runtime inventory contains missing item revisions." );
        }

        if ( instances.Length > 0 )
        {
            int runtimeCampaignId = instances[ 0 ].CampaignId;
            bool definitionsAreVisible = definitions.All( definition =>
                ( definition.Scope == ItemCatalogScope.Global ) ||
                ( ( definition.Scope == ItemCatalogScope.Campaign ) &&
                  ( definition.CampaignId == runtimeCampaignId ) ) );
            if ( !definitionsAreVisible )
            {
                throw new InvalidOperationException(
                    "Character runtime inventory references an item definition from another campaign." );
            }
        }

        Dictionary<int, ItemConfiguration> configurationsById = configurations
            .ToDictionary( configuration => configuration.Id );
        Dictionary<int, ItemDefinition> definitionsByRevisionId = definitions
            .SelectMany( definition => definition.Revisions
                .Where( revision => revisionIds.Contains( revision.Id ) )
                .Select( revision => new
                {
                    revision.Id,
                    Definition = definition,
                } ) )
            .ToDictionary( item => item.Id, item => item.Definition );
        Dictionary<Guid, ItemInstance> instancesByKey = instances
            .ToDictionary( instance => instance.InstanceKey );
        RuntimeEquipmentLine[] runtimeLines = character.RuntimeEquipmentItems
            .Select( reference =>
            {
                ItemInstance instance = instancesByKey[ reference.ItemInstanceKey ];
                ItemConfiguration configuration = configurationsById[ instance.ItemConfigurationId ];
                ItemDefinition definition = definitionsByRevisionId[ configuration.ItemRevisionId ];
                return new RuntimeEquipmentLine(
                    definition.Key,
                    configuration.ItemRevisionId,
                    instance.Quantity,
                    reference.IsEquipped ? instance.Quantity : 0 );
            } )
            .Where( line => line.Quantity > 0 )
            .ToArray();
        IReadOnlyList<CharacterEquipmentItem> equipmentItems = runtimeLines
            .GroupBy( line => line.EquipmentId, StringComparer.Ordinal )
            .Select( group => new CharacterEquipmentItem(
                group.Key,
                group.Sum( line => line.Quantity ),
                group.Sum( line => line.EquippedQuantity ) ) )
            .OrderBy( item => item.EquipmentId, StringComparer.Ordinal )
            .ToArray();
        Dictionary<string, ItemRevision> exactRevisions = runtimeLines
            .GroupBy( line => line.EquipmentId, StringComparer.Ordinal )
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    int[] exactRevisionIds = group
                        .Select( line => line.ItemRevisionId )
                        .Distinct()
                        .ToArray();
                    if ( exactRevisionIds.Length != 1 )
                    {
                        throw new InvalidOperationException(
                            $"Combat-card v1 cannot combine multiple revisions of '{group.Key}'." );
                    }

                    return revisionsById[ exactRevisionIds[ 0 ] ];
                },
                StringComparer.Ordinal );
        return _fallbackReader.ReadExactRevisions(
            character,
            proficiencies,
            equipmentItems,
            exactRevisions,
            campaignId );
    }

    private void ValidateOwnership(
        DraftCharacter character,
        IReadOnlyCollection<ItemInstance> instances,
        int? campaignId )
    {
        Guid[] containerKeys = instances
            .Select( instance => instance.CurrentContainerKey )
            .Distinct()
            .ToArray();
        InventoryContainer[] containers = _inventoryDbContext.Containers
            .AsNoTracking()
            .Where( container => containerKeys.Contains( container.ContainerKey ) )
            .ToArray();
        bool ownsAllContainers = ( containers.Length == containerKeys.Length ) &&
            containers.All( container =>
                ( container.OwnerKind == InventoryContainerOwnerKind.Character ) &&
                ( container.OwnerId == character.Id ) &&
                ( !campaignId.HasValue || ( container.CampaignId == campaignId.Value ) ) );
        bool scopesAllInstances = instances.All( instance =>
            !campaignId.HasValue || ( instance.CampaignId == campaignId.Value ) );
        bool hasSingleCampaign = instances
            .Select( instance => instance.CampaignId )
            .Distinct()
            .Count() <= 1;
        if ( !ownsAllContainers || !scopesAllInstances || !hasSingleCampaign )
        {
            throw new InvalidOperationException(
                "Character runtime inventory is outside the requested ownership scope." );
        }
    }

    private sealed record RuntimeEquipmentLine(
        string EquipmentId,
        int ItemRevisionId,
        int Quantity,
        int EquippedQuantity );
}

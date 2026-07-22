using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Equipment;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;

namespace Pathfinder.Web.Integration;

public sealed class CompletedCharacterInventoryMigrationService
{
    private readonly CharacterManagementDbContext _characterDbContext;
    private readonly CampaignManagementDbContext _campaignDbContext;
    private readonly ItemCatalogDbContext _itemCatalogDbContext;
    private readonly InventoryDbContext _inventoryDbContext;
    private readonly IEquipmentRepository _equipmentRepository;

    public CompletedCharacterInventoryMigrationService(
        CharacterManagementDbContext characterDbContext,
        CampaignManagementDbContext campaignDbContext,
        ItemCatalogDbContext itemCatalogDbContext,
        InventoryDbContext inventoryDbContext,
        IEquipmentRepository equipmentRepository )
    {
        _characterDbContext = characterDbContext;
        _campaignDbContext = campaignDbContext;
        _itemCatalogDbContext = itemCatalogDbContext;
        _inventoryDbContext = inventoryDbContext;
        _equipmentRepository = equipmentRepository;
    }

    public async Task<CompletedCharacterInventoryMigrationResult> MigrateAsync(
        int characterId,
        DateTimeOffset migratedAtUtc,
        CancellationToken cancellationToken )
    {
        if ( migratedAtUtc.Offset != TimeSpan.Zero )
        {
            throw new InvalidOperationException( "Inventory migration timestamp must use UTC." );
        }

        DraftCharacter? character = await _characterDbContext.Character
            .SingleOrDefaultAsync( item => item.Id == characterId, cancellationToken );
        if ( character is null )
        {
            return CompletedCharacterInventoryMigrationResult.NotFound( characterId );
        }

        if ( character.CreationStatus != CharacterCreationStatus.Completed )
        {
            return CompletedCharacterInventoryMigrationResult.NotCompleted( characterId );
        }

        if ( character.HasRuntimeInventory )
        {
            return CompletedCharacterInventoryMigrationResult.AlreadyMigrated(
                characterId,
                character.RuntimeEquipmentItems.Count );
        }

        int[] campaignIds = await _campaignDbContext.CampaignPartyCharacters
            .Where( assignment => assignment.CharacterId == characterId )
            .Join(
                _campaignDbContext.CampaignParties,
                assignment => assignment.CampaignPartyId,
                party => party.Id,
                ( assignment, party ) => party.CampaignId )
            .Distinct()
            .ToArrayAsync( cancellationToken );
        if ( campaignIds.Length != 1 )
        {
            return CompletedCharacterInventoryMigrationResult.InvalidCampaignAssignment(
                characterId,
                campaignIds.Length );
        }

        int campaignId = campaignIds[ 0 ];
        MigrationEquipmentLine[] orderedEquipment = character.StartingEquipmentItems
            .GroupBy( item => item.EquipmentId, StringComparer.Ordinal )
            .Select( group => new MigrationEquipmentLine(
                group.Key,
                group.Sum( item => item.PurchaseQuantity ),
                group.Sum( item => item.EquippedQuantity ) ) )
            .OrderBy( item => item.EquipmentId, StringComparer.Ordinal )
            .ToArray();
        if ( orderedEquipment.Any( item =>
            ( item.PurchaseQuantity < 0 ) ||
            ( item.EquippedQuantity < 0 ) ||
            ( item.EquippedQuantity > item.PurchaseQuantity ) ) )
        {
            throw new InvalidOperationException(
                "Completed character equipment quantities are inconsistent." );
        }

        Dictionary<string, int> configurationIds = new Dictionary<string, int>(
            StringComparer.Ordinal );
        foreach ( MigrationEquipmentLine equipment in orderedEquipment )
        {
            int? itemConfigurationId = await ResolveConfigurationAsync(
                equipment.EquipmentId,
                campaignId,
                migratedAtUtc,
                cancellationToken );
            if ( !itemConfigurationId.HasValue )
            {
                return CompletedCharacterInventoryMigrationResult.MissingConfiguration(
                    characterId,
                    equipment.EquipmentId );
            }

            configurationIds.Add( equipment.EquipmentId, itemConfigurationId.Value );
        }

        InventoryContainer container = await GetOrCreateContainerAsync(
            characterId,
            campaignId,
            migratedAtUtc,
            cancellationToken );
        List<CharacterRuntimeEquipmentItem> runtimeItems = [];
        foreach ( MigrationEquipmentLine equipment in orderedEquipment )
        {
            for ( int ordinal = 0; ordinal < equipment.PurchaseQuantity; ordinal++ )
            {
                Guid instanceKey = CreateDeterministicKey(
                    $"item:{campaignId}:{characterId}:{equipment.EquipmentId}:{ordinal}" );
                ItemInstance? instance = await _inventoryDbContext.ItemInstances
                    .SingleOrDefaultAsync(
                        item => item.InstanceKey == instanceKey,
                        cancellationToken );
                if ( instance is null )
                {
                    instance = ItemInstance.Create(
                        instanceKey,
                        campaignId,
                        configurationIds[ equipment.EquipmentId ],
                        container,
                        null,
                        migratedAtUtc );
                    _inventoryDbContext.ItemInstances.Add( instance );
                }
                else if ( ( instance.CampaignId != campaignId ) ||
                          ( instance.ItemConfigurationId != configurationIds[ equipment.EquipmentId ] ) ||
                          ( instance.CurrentContainerKey != container.ContainerKey ) )
                {
                    throw new InvalidOperationException(
                        $"Existing migrated item instance '{instanceKey}' does not match its source equipment." );
                }

                runtimeItems.Add( new CharacterRuntimeEquipmentItem(
                    instanceKey,
                    ordinal < equipment.EquippedQuantity ) );
            }
        }

        await _inventoryDbContext.SaveChangesAsync( cancellationToken );
        character.SetRuntimeInventory( runtimeItems );
        await _characterDbContext.SaveChangesAsync( cancellationToken );
        return CompletedCharacterInventoryMigrationResult.Migrated(
            characterId,
            campaignId,
            runtimeItems.Count );
    }

    public async Task<IReadOnlyCollection<CompletedCharacterInventoryMigrationResult>> MigratePendingAsync(
        DateTimeOffset migratedAtUtc,
        CancellationToken cancellationToken )
    {
        int[] characterIds = await _characterDbContext.Character
            .Where( character =>
                ( character.CreationStatus == CharacterCreationStatus.Completed ) &&
                !character.HasRuntimeInventory )
            .Select( character => character.Id )
            .ToArrayAsync( cancellationToken );
        List<CompletedCharacterInventoryMigrationResult> results = [];
        foreach ( int characterId in characterIds )
        {
            results.Add( await MigrateAsync(
                characterId,
                migratedAtUtc,
                cancellationToken ) );
        }

        return results;
    }

    private async Task<InventoryContainer> GetOrCreateContainerAsync(
        int characterId,
        int campaignId,
        DateTimeOffset createdAtUtc,
        CancellationToken cancellationToken )
    {
        InventoryContainer? container = await _inventoryDbContext.Containers
            .SingleOrDefaultAsync(
                item =>
                    ( item.CampaignId == campaignId ) &&
                    ( item.OwnerKind == InventoryContainerOwnerKind.Character ) &&
                    ( item.OwnerId == characterId ),
                cancellationToken );
        if ( container is not null )
        {
            return container;
        }

        container = InventoryContainer.CreateRoot(
            CreateDeterministicKey( $"container:{campaignId}:{characterId}" ),
            campaignId,
            InventoryContainerOwnerKind.Character,
            characterId,
            createdAtUtc );
        _inventoryDbContext.Containers.Add( container );
        return container;
    }

    private async Task<int?> ResolveConfigurationAsync(
        string equipmentId,
        int campaignId,
        DateTimeOffset createdAtUtc,
        CancellationToken cancellationToken )
    {
        ItemDefinition? definition = await _itemCatalogDbContext.ItemDefinitions
            .Include( item => item.Revisions )
            .Where( item =>
                ( item.Key == equipmentId ) &&
                ( ( item.Scope == ItemCatalogScope.Global ) ||
                  ( ( item.Scope == ItemCatalogScope.Campaign ) &&
                    ( item.CampaignId == campaignId ) ) ) )
            .OrderByDescending( item => item.Scope == ItemCatalogScope.Campaign )
            .FirstOrDefaultAsync( cancellationToken );
        ItemRevision? revision = definition?.Revisions
            .SingleOrDefault( item => item.Status == ItemRevisionStatus.Published );
        if ( ( revision is null ) && ( definition is null ) )
        {
            definition = await BootstrapDefinitionAsync(
                equipmentId,
                createdAtUtc,
                cancellationToken );
            revision = definition.Revisions
                .Single( item => item.Status == ItemRevisionStatus.Published );
        }

        if ( revision is null )
        {
            return null;
        }

        ItemConfiguration? configuration = await _itemCatalogDbContext.ItemConfigurations
            .SingleOrDefaultAsync(
                item =>
                    ( item.ItemRevisionId == revision.Id ) &&
                    ( item.Size == ItemSize.Medium ) &&
                    ( item.MaterialType == ItemMaterialType.Standard ) &&
                    ( item.MaterialGrade == ItemMaterialGrade.Standard ) &&
                    !item.PermanentUpgrades.Any(),
                cancellationToken );
        if ( configuration is not null )
        {
            return configuration.Id;
        }

        configuration = ItemConfiguration.Create(
            revision.Id,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [],
            createdAtUtc );
        _itemCatalogDbContext.ItemConfigurations.Add( configuration );
        await _itemCatalogDbContext.SaveChangesAsync( cancellationToken );
        return configuration.Id;
    }

    private async Task<ItemDefinition> BootstrapDefinitionAsync(
        string equipmentId,
        DateTimeOffset createdAtUtc,
        CancellationToken cancellationToken )
    {
        EquipmentDefinition equipment = _equipmentRepository.GetEquipment( equipmentId );
        ItemDefinition definition = ItemDefinition.CreateGlobal(
            equipment.Id,
            createdAtUtc );
        ItemRevision revision = definition.CreateRevision(
            equipment.Name,
            String.Empty,
            0,
            equipment.PriceCopper,
            equipment.BulkTenths / 10m,
            CreateRules( equipment ),
            createdAtUtc );
        definition.PublishRevision( revision.RevisionNumber, createdAtUtc );
        _itemCatalogDbContext.ItemDefinitions.Add( definition );
        await _itemCatalogDbContext.SaveChangesAsync( cancellationToken );
        return definition;
    }

    private static ItemRevisionRules CreateRules( EquipmentDefinition equipment )
    {
        return equipment.Category switch
        {
            EquipmentCategory.Weapon => CreateWeaponRules( equipment ),
            EquipmentCategory.Armor => CreateArmorRules( equipment ),
            EquipmentCategory.Shield => CreateShieldRules( equipment ),
            EquipmentCategory.Ammunition => ItemRevisionRules.Create(
                ItemCategory.Ammunition,
                equipment: EquipmentComponent.Create( EquipmentUsage.Stored, 0 ),
                consumption: ConsumptionComponent.Create(
                    ConsumptionMode.ConsumeAmmunition,
                    1 ) ),
            _ => ItemRevisionRules.Create(
                ItemCategory.OtherEquipment,
                equipment: EquipmentComponent.Create( EquipmentUsage.Stored, 0 ) ),
        };
    }

    private static ItemRevisionRules CreateWeaponRules( EquipmentDefinition equipment )
    {
        EquipmentWeaponStatistics weapon = equipment.Weapon
            ?? throw new InvalidOperationException( "Starting weapon statistics are missing." );
        int hands = weapon.Hands.StartsWith( "2", StringComparison.Ordinal ) ? 2 : 1;
        return ItemRevisionRules.Create(
            ItemCategory.Weapon,
            attacks:
            [
                AttackComponent.Create(
                    equipment.Name,
                    1,
                    ( DamageDieSize )weapon.DamageDie,
                    Enum.Parse<ItemDamageType>( weapon.DamageType ),
                    hands,
                    weapon.RangeFeet ),
            ],
            equipment: EquipmentComponent.Create( EquipmentUsage.Held, hands ) );
    }

    private static ItemRevisionRules CreateArmorRules( EquipmentDefinition equipment )
    {
        EquipmentArmorStatistics armor = equipment.Armor
            ?? throw new InvalidOperationException( "Starting armor statistics are missing." );
        ArmorCategory category = armor.Category switch
        {
            EquipmentArmorCategory.Unarmored => ArmorCategory.Unarmored,
            EquipmentArmorCategory.Light => ArmorCategory.Light,
            EquipmentArmorCategory.Medium => ArmorCategory.Medium,
            _ => throw new InvalidOperationException( "Starting armor category is invalid." ),
        };
        return ItemRevisionRules.Create(
            ItemCategory.Armor,
            armor: ArmorComponent.Create(
                category,
                armor.ArmorClassBonus,
                armor.DexterityCap,
                armor.CheckPenalty,
                armor.SpeedPenaltyFeet,
                armor.StrengthThreshold ),
            equipment: EquipmentComponent.Create( EquipmentUsage.Worn, 0 ) );
    }

    private static ItemRevisionRules CreateShieldRules( EquipmentDefinition equipment )
    {
        EquipmentShieldStatistics shield = equipment.Shield
            ?? throw new InvalidOperationException( "Starting shield statistics are missing." );
        return ItemRevisionRules.Create(
            ItemCategory.Shield,
            attacks:
            [
                AttackComponent.Create(
                    $"{equipment.Name} Bash",
                    1,
                    DamageDieSize.D4,
                    ItemDamageType.Bludgeoning,
                    1 ),
            ],
            shield: ShieldComponent.Create( shield.ArmorClassBonus ),
            equipment: EquipmentComponent.Create( EquipmentUsage.Held, 1 ),
            durability: DurabilityComponent.Create(
                shield.Hardness,
                shield.HitPoints,
                Math.Max( 1, shield.HitPoints / 2 ) ) );
    }

    private static Guid CreateDeterministicKey( string value )
    {
        byte[] hash = SHA256.HashData( Encoding.UTF8.GetBytes( value ) );
        return new Guid( hash.AsSpan( 0, 16 ) );
    }

    private sealed record MigrationEquipmentLine(
        string EquipmentId,
        int PurchaseQuantity,
        int EquippedQuantity );
}

public sealed record CompletedCharacterInventoryMigrationResult(
    int CharacterId,
    string Status,
    int? CampaignId,
    int MigratedItemCount,
    string? MissingEquipmentId,
    int CampaignAssignmentCount )
{
    public static CompletedCharacterInventoryMigrationResult Migrated(
        int characterId,
        int campaignId,
        int migratedItemCount ) =>
        new CompletedCharacterInventoryMigrationResult(
            characterId,
            "Migrated",
            campaignId,
            migratedItemCount,
            null,
            1 );

    public static CompletedCharacterInventoryMigrationResult AlreadyMigrated(
        int characterId,
        int migratedItemCount ) =>
        new CompletedCharacterInventoryMigrationResult(
            characterId,
            "AlreadyMigrated",
            null,
            migratedItemCount,
            null,
            1 );

    public static CompletedCharacterInventoryMigrationResult NotFound( int characterId ) =>
        new CompletedCharacterInventoryMigrationResult(
            characterId,
            "NotFound",
            null,
            0,
            null,
            0 );

    public static CompletedCharacterInventoryMigrationResult NotCompleted( int characterId ) =>
        new CompletedCharacterInventoryMigrationResult(
            characterId,
            "NotCompleted",
            null,
            0,
            null,
            0 );

    public static CompletedCharacterInventoryMigrationResult InvalidCampaignAssignment(
        int characterId,
        int campaignAssignmentCount ) =>
        new CompletedCharacterInventoryMigrationResult(
            characterId,
            "InvalidCampaignAssignment",
            null,
            0,
            null,
            campaignAssignmentCount );

    public static CompletedCharacterInventoryMigrationResult MissingConfiguration(
        int characterId,
        string equipmentId ) =>
        new CompletedCharacterInventoryMigrationResult(
            characterId,
            "MissingConfiguration",
            null,
            0,
            equipmentId,
            1 );
}

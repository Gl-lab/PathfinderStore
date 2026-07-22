using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Equipment;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.Web.Integration;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class CompletedCharacterInventoryMigrationServiceTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 18, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task MigrationCreatesDistinctInstancesAndPreservesEquippedState()
    {
        DbContextOptions<CharacterManagementDbContext> characterOptions =
            new DbContextOptionsBuilder<CharacterManagementDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        DbContextOptions<CampaignManagementDbContext> campaignOptions =
            new DbContextOptionsBuilder<CampaignManagementDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        DbContextOptions<ItemCatalogDbContext> catalogOptions =
            new DbContextOptionsBuilder<ItemCatalogDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        DbContextOptions<InventoryDbContext> inventoryOptions =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        await using CharacterManagementDbContext characterContext =
            new CharacterManagementDbContext( characterOptions );
        await using CampaignManagementDbContext campaignContext =
            new CampaignManagementDbContext( campaignOptions );
        await using ItemCatalogDbContext catalogContext =
            new ItemCatalogDbContext( catalogOptions );
        await using InventoryDbContext inventoryContext =
            new InventoryDbContext( inventoryOptions );
        DraftCharacter character = CreateCompletedCharacter();
        characterContext.Character.Add( character );
        await characterContext.SaveChangesAsync();
        Campaign campaign = Campaign.Create( "Abomination Vaults", 11, _createdAtUtc );
        campaign.AssignRole( 11, 11, CampaignMembershipRole.Player, _createdAtUtc );
        campaign.CreateParty( 11, "Heroes", _createdAtUtc );
        campaign.AssignCharacterToActiveParty(
            11,
            character.Id,
            11,
            _createdAtUtc );
        campaignContext.Campaigns.Add( campaign );
        await campaignContext.SaveChangesAsync();
        CompletedCharacterInventoryMigrationService service = new CompletedCharacterInventoryMigrationService(
            characterContext,
            campaignContext,
            catalogContext,
            inventoryContext,
            new EquipmentRepository() );

        CompletedCharacterInventoryMigrationResult result = await service.MigrateAsync(
            character.Id,
            _createdAtUtc.AddMinutes( 2 ),
            CancellationToken.None );

        Assert.Equal( "Migrated", result.Status );
        Assert.Equal( 2, result.MigratedItemCount );
        Assert.True( character.HasRuntimeInventory );
        Assert.Equal( 2, character.RuntimeEquipmentItems.Count );
        Assert.Single( character.RuntimeEquipmentItems.Where( item => item.IsEquipped ) );
        InventoryContainer container = Assert.Single( inventoryContext.Containers );
        Assert.Equal( InventoryContainerOwnerKind.Character, container.OwnerKind );
        Assert.Equal( character.Id, container.OwnerId );
        ItemInstance[] instances = await inventoryContext.ItemInstances.ToArrayAsync();
        Assert.Equal( 2, instances.Length );
        Assert.Equal( 2, instances.Select( item => item.InstanceKey ).Distinct().Count() );
        Assert.All( instances, item => Assert.Equal( container.ContainerKey, item.CurrentContainerKey ) );
        Assert.All( instances, item => Assert.True( item.ItemConfigurationId > 0 ) );
        ItemDefinition definition = await catalogContext.ItemDefinitions
            .Include( item => item.Revisions )
            .SingleAsync( item => item.Key == "equipment.dagger" );
        Assert.Equal(
            ItemRevisionStatus.Published,
            Assert.Single( definition.Revisions ).Status );
        Assert.Single( catalogContext.ItemConfigurations );

        CompletedCharacterInventoryMigrationResult replay = await service.MigrateAsync(
            character.Id,
            _createdAtUtc.AddMinutes( 3 ),
            CancellationToken.None );
        Assert.Equal( "AlreadyMigrated", replay.Status );
        Assert.Equal( 2, await inventoryContext.ItemInstances.CountAsync() );
    }

    private static DraftCharacter CreateCompletedCharacter()
    {
        DraftCharacter character = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human,
            gender: CharacterGender.Male );
        typeof( DraftCharacter )
            .GetProperty( nameof( DraftCharacter.StartingEquipmentItems ) )!
            .SetValue(
                character,
                new CharacterEquipmentItem[]
                {
                    new CharacterEquipmentItem( "equipment.dagger", 2, 1 ),
                } );
        character.FinalizeCreation( _createdAtUtc );
        return character;
    }

}

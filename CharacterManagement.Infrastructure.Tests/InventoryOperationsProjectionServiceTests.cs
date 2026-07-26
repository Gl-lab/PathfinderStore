using CharacterManagement.Infrastructure.Tests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Domain.Campaigns;
using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Application.Access;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Inventory.Application.Audit;
using Pathfinder.Inventory.Application.Storage;
using Pathfinder.Inventory.Domain.Audit;
using Pathfinder.Inventory.Domain.Containers;
using Pathfinder.Inventory.Domain.Items;
using Pathfinder.Inventory.Domain.Transfers;
using Pathfinder.Inventory.Infrastructure.Data;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.Web.Integration;

namespace Pathfinder.CharacterManagement.Infrastructure.Tests;

public sealed class InventoryOperationsProjectionServiceTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 26, 16, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task PendingGiftAndExchangeProjectionsAreScopedAndResolved()
    {
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        DraftCharacter sourceCharacter = DraftCharacter.Create(
            1,
            "Valeros",
            AncestryType.Human );
        DraftCharacter destinationCharacter = DraftCharacter.Create(
            2,
            "Ezren",
            AncestryType.Human );
        characterDbContext.Character.AddRange( sourceCharacter, destinationCharacter );
        await characterDbContext.SaveChangesAsync();
        ItemConfiguration configuration = await AddCatalogItemAsync( catalogDbContext, 42 );
        InventoryContainer sourceContainer = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            42,
            InventoryContainerOwnerKind.Character,
            sourceCharacter.Id,
            _now );
        InventoryContainer destinationContainer = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            42,
            InventoryContainerOwnerKind.Character,
            destinationCharacter.Id,
            _now );
        ItemInstance sourceItem = ItemInstance.Create(
            Guid.NewGuid(),
            42,
            configuration.Id,
            sourceContainer,
            null,
            _now );
        ItemInstance destinationItem = ItemInstance.Create(
            Guid.NewGuid(),
            42,
            configuration.Id,
            destinationContainer,
            "Ezren's blade",
            _now );
        PartyGift incomingGift = PartyGift.Create(
            Guid.NewGuid(),
            42,
            3,
            sourceCharacter.Id,
            destinationCharacter.Id,
            sourceItem.InstanceKey,
            sourceItem.Version,
            _now,
            _now.AddDays( 1 ) );
        PartyGift unrelatedGift = PartyGift.Create(
            Guid.NewGuid(),
            42,
            3,
            destinationCharacter.Id,
            sourceCharacter.Id,
            destinationItem.InstanceKey,
            destinationItem.Version,
            _now.AddMinutes( 1 ),
            _now.AddDays( 1 ) );
        PartyExchange exchange = PartyExchange.Create(
            Guid.NewGuid(),
            42,
            3,
            sourceCharacter.Id,
            destinationCharacter.Id,
            [
                new PartyExchangeLineRequest(
                    sourceCharacter.Id,
                    sourceItem.InstanceKey,
                    sourceItem.Version ),
                new PartyExchangeLineRequest(
                    destinationCharacter.Id,
                    destinationItem.InstanceKey,
                    destinationItem.Version ),
            ],
            _now,
            _now.AddHours( 1 ) );
        inventoryDbContext.AddRange(
            sourceContainer,
            destinationContainer,
            sourceItem,
            destinationItem,
            incomingGift,
            unrelatedGift,
            exchange );
        await inventoryDbContext.SaveChangesAsync();
        InventoryOperationsProjectionService service = CreateService(
            campaignDbContext,
            characterDbContext,
            inventoryDbContext,
            catalogDbContext,
            new CharacterCampaignAccess( true, true ) );

        IReadOnlyCollection<PartyGiftProjectionDto> gifts = await service.GetGiftsAsync(
            42,
            destinationCharacter.Id,
            PartyGiftRole.Incoming,
            PartyGiftStatus.Pending,
            7,
            CancellationToken.None );
        IReadOnlyCollection<PartyExchangeProjectionDto> exchanges =
            await service.GetExchangesAsync(
                42,
                destinationCharacter.Id,
                PartyExchangeStatus.Pending,
                7,
                CancellationToken.None );

        PartyGiftProjectionDto gift = Assert.Single( gifts );
        Assert.Equal( incomingGift.GiftKey, gift.Gift.GiftKey );
        Assert.Equal( "Valeros", gift.SourceCharacter.Name );
        Assert.Equal( "Ezren", gift.DestinationCharacter.Name );
        Assert.Equal( "Longsword", gift.Item.Name );
        PartyExchangeProjectionDto exchangeDto = Assert.Single( exchanges );
        Assert.Equal( 2, exchangeDto.Items.Count );
        Assert.Contains(
            exchangeDto.Items,
            item => item.Item.Name == "Ezren's blade" );
    }

    [Fact]
    public async Task PartyStorageProjectionIncludesPolicyDepositorAndRecentAudit()
    {
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        DraftCharacter character = DraftCharacter.Create( 1, "Kyra", AncestryType.Human );
        characterDbContext.Character.Add( character );
        await characterDbContext.SaveChangesAsync();
        Campaign campaign = Campaign.Create( "Abomination Vaults", 42, _now );
        CampaignInvitation invitation = campaign.Invite(
            42,
            7,
            _now.AddSeconds( 30 ) );
        campaign.AcceptInvitation(
            invitation.Id,
            7,
            _now.AddSeconds( 45 ) );
        campaign.CreateParty( 42, "Heroes", _now.AddMinutes( 1 ) );
        campaign.AssignCharacterToActiveParty(
            42,
            character.Id,
            7,
            _now.AddMinutes( 2 ) );
        campaign.SetActivePartyStorageAccessPolicy(
            42,
            CampaignPartyStorageAccessPolicy.FreeForMembers );
        campaignDbContext.Campaigns.Add( campaign );
        await campaignDbContext.SaveChangesAsync();
        CampaignParty party = Assert.Single( campaign.Parties );
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        ItemConfiguration configuration = await AddCatalogItemAsync(
            catalogDbContext,
            campaign.Id );
        InventoryContainer characterContainer = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            campaign.Id,
            InventoryContainerOwnerKind.Character,
            character.Id,
            _now );
        InventoryContainer partyContainer = InventoryContainer.CreateRoot(
            Guid.NewGuid(),
            campaign.Id,
            InventoryContainerOwnerKind.Party,
            party.Id,
            _now );
        ItemInstance item = ItemInstance.Create(
            Guid.NewGuid(),
            campaign.Id,
            configuration.Id,
            characterContainer,
            null,
            _now );
        Guid operationId = Guid.NewGuid();
        item.MoveTo(
            partyContainer,
            "party-storage-deposit",
            item.Version,
            operationId,
            "user:42",
            _now.AddMinutes( 3 ) );
        InventoryAuditEntry audit = InventoryAuditFactory.CreatePlayerAction(
            campaign.Id,
            operationId,
            InventoryAuditActionKind.PartyStorageDeposited,
            42,
            "Item deposited into party storage.",
            item.InstanceKey,
            null,
            _now.AddMinutes( 3 ) );
        inventoryDbContext.AddRange(
            characterContainer,
            partyContainer,
            item,
            audit );
        await inventoryDbContext.SaveChangesAsync();
        InventoryOperationsProjectionService service = CreateService(
            campaignDbContext,
            characterDbContext,
            inventoryDbContext,
            catalogDbContext,
            CharacterCampaignAccess.Denied );

        PartyStorageProjectionDto result = await service.GetPartyStorageAsync(
            campaign.Id,
            42,
            CancellationToken.None );

        Assert.Equal( PartyStorageWithdrawalPolicy.FreeForMembers, result.AccessPolicy );
        PartyStorageItemProjectionDto storageItem = Assert.Single( result.Items );
        Assert.Equal( "Kyra", storageItem.DepositedBy?.Name );
        Assert.Equal( _now.AddMinutes( 3 ), storageItem.DepositedAtUtc );
        PartyStorageOperationProjectionDto operation = Assert.Single(
            result.RecentOperations );
        Assert.Equal( InventoryAuditActionKind.PartyStorageDeposited, operation.Kind );
        Assert.Equal( "Kyra", operation.Character?.Name );
        Assert.Equal( "Longsword", operation.Item.Name );
    }

    [Fact]
    public async Task OperationsProjectionRejectsCharacterWithoutAccess()
    {
        await using CampaignManagementDbContext campaignDbContext = CreateCampaignContext();
        await using CharacterManagementDbContext characterDbContext =
            TestCharacterManagementDbContextFactory.Create();
        await using InventoryDbContext inventoryDbContext = CreateInventoryContext();
        await using ItemCatalogDbContext catalogDbContext = CreateCatalogContext();
        InventoryOperationsProjectionService service = CreateService(
            campaignDbContext,
            characterDbContext,
            inventoryDbContext,
            catalogDbContext,
            CharacterCampaignAccess.Denied );

        await Assert.ThrowsAsync<InventoryOperationsAccessDeniedException>(
            () => service.GetGiftsAsync(
                42,
                101,
                PartyGiftRole.Incoming,
                PartyGiftStatus.Pending,
                7,
                CancellationToken.None ) );
    }

    private static InventoryOperationsProjectionService CreateService(
        CampaignManagementDbContext campaignDbContext,
        CharacterManagementDbContext characterDbContext,
        InventoryDbContext inventoryDbContext,
        ItemCatalogDbContext catalogDbContext,
        CharacterCampaignAccess access )
    {
        return new InventoryOperationsProjectionService(
            campaignDbContext,
            characterDbContext,
            inventoryDbContext,
            new InventoryItemCatalogProjectionReader( catalogDbContext ),
            new StubCharacterAccessPolicy( access ) );
    }

    private static async Task<ItemConfiguration> AddCatalogItemAsync(
        ItemCatalogDbContext dbContext,
        int campaignId )
    {
        ItemDefinition definition = ItemDefinition.CreateForCampaign(
            "equipment.longsword",
            campaignId,
            _now );
        ItemRevision revision = definition.CreateRevision(
            "Longsword",
            "A steel sword.",
            1,
            100,
            1m,
            ItemRevisionRules.Create(
                ItemCategory.Weapon,
                attacks:
                [
                    AttackComponent.Create(
                        "Longsword",
                        1,
                        DamageDieSize.D8,
                        ItemDamageType.Slashing,
                        1 ),
                ] ),
            _now );
        definition.PublishRevision( 1, _now );
        dbContext.ItemDefinitions.Add( definition );
        await dbContext.SaveChangesAsync();
        ItemConfiguration configuration = ItemConfiguration.Create(
            campaignId,
            revision.Id,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [],
            _now );
        dbContext.ItemConfigurations.Add( configuration );
        await dbContext.SaveChangesAsync();
        return configuration;
    }

    private static CampaignManagementDbContext CreateCampaignContext()
    {
        DbContextOptions<CampaignManagementDbContext> options =
            new DbContextOptionsBuilder<CampaignManagementDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new CampaignManagementDbContext( options );
    }

    private static InventoryDbContext CreateInventoryContext()
    {
        DbContextOptions<InventoryDbContext> options =
            new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new InventoryDbContext( options );
    }

    private static ItemCatalogDbContext CreateCatalogContext()
    {
        DbContextOptions<ItemCatalogDbContext> options =
            new DbContextOptionsBuilder<ItemCatalogDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new ItemCatalogDbContext( options );
    }

    private sealed class StubCharacterAccessPolicy : ICharacterCampaignAccessPolicy
    {
        private readonly CharacterCampaignAccess _access;

        public StubCharacterAccessPolicy( CharacterCampaignAccess access )
        {
            _access = access;
        }

        public Task<CharacterCampaignAccess> GetAccessAsync(
            int campaignId,
            int userId,
            int characterId,
            CancellationToken cancellationToken ) => Task.FromResult( _access );
    }
}
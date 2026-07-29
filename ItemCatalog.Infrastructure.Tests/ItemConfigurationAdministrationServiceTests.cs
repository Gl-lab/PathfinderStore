using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pathfinder.ItemCatalog.Application.Administration;
using Pathfinder.ItemCatalog.Application.Configurations;
using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Domain.Configurations;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Configurations;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.ItemCatalog.Infrastructure.Items;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.ItemCatalog.Infrastructure.Tests;

public sealed class ItemConfigurationAdministrationServiceTests
{
    private const int CampaignId = 42;
    private const int GameMasterId = 7;

    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 22, 12, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task CreateStoresCampaignScopedConfiguration()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        ItemDefinition definition = await SeedDefinitionAsync( dbContext, publish: true );
        ItemConfigurationAdministrationService service = CreateService( dbContext );

        ItemConfigurationDto result = await service.CreateAsync(
            CreateRequest( definition.Id ),
            CancellationToken.None );

        Assert.True( result.WasCreated );
        Assert.Equal( CampaignId, result.CampaignId );
        Assert.Equal( ItemSize.Medium, result.Size );
        ItemConfiguration persisted = Assert.Single( dbContext.ItemConfigurations );
        Assert.Equal( result.ItemConfigurationId, persisted.Id );
        Assert.Equal( result.ConfigurationKey, persisted.ConfigurationKey );
    }

    [Fact]
    public async Task CreateReturnsExistingConfigurationForSameShape()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        ItemDefinition definition = await SeedDefinitionAsync( dbContext, publish: true );
        ItemConfigurationAdministrationService service = CreateService( dbContext );
        ItemConfigurationDto first = await service.CreateAsync(
            CreateRequest( definition.Id ),
            CancellationToken.None );

        ItemConfigurationDto second = await service.CreateAsync(
            CreateRequest( definition.Id ),
            CancellationToken.None );

        Assert.True( first.WasCreated );
        Assert.False( second.WasCreated );
        Assert.Equal( first.ItemConfigurationId, second.ItemConfigurationId );
        Assert.Equal( first.ConfigurationKey, second.ConfigurationKey );
        Assert.Single( dbContext.ItemConfigurations );
    }

    [Fact]
    public async Task CreateAcceptsPermanentUpgradesAndDeduplicatesByKey()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        ItemDefinition definition = await SeedDefinitionAsync( dbContext, publish: true );
        ItemConfigurationAdministrationService service = CreateService( dbContext );
        CreateItemConfigurationRequest baseRequest = CreateRequest( definition.Id );

        ItemConfigurationDto upgraded = await service.CreateAsync(
            baseRequest with
            {
                PermanentUpgrades =
                [
                    PermanentUpgrade.Create(
                        "rune.striking",
                        PermanentUpgradeKind.StrikingRune,
                        1,
                        PermanentUpgradeVisibility.Public ),
                ],
            },
            CancellationToken.None );
        ItemConfigurationDto upgradedReplay = await service.CreateAsync(
            baseRequest with
            {
                PermanentUpgrades =
                [
                    PermanentUpgrade.Create(
                        "rune.striking",
                        PermanentUpgradeKind.StrikingRune,
                        1,
                        PermanentUpgradeVisibility.Public ),
                ],
            },
            CancellationToken.None );
        ItemConfigurationDto plain = await service.CreateAsync(
            baseRequest,
            CancellationToken.None );

        Assert.True( upgraded.WasCreated );
        Assert.False( upgradedReplay.WasCreated );
        Assert.Equal( upgraded.ItemConfigurationId, upgradedReplay.ItemConfigurationId );
        Assert.Equal( "rune.striking", Assert.Single( upgradedReplay.PermanentUpgrades ).Code );
        Assert.True( plain.WasCreated );
        Assert.NotEqual( upgraded.ItemConfigurationId, plain.ItemConfigurationId );
        Assert.Equal( 2, dbContext.ItemConfigurations.Count() );
    }

    [Fact]
    public async Task CreateFailsForCampaignDraftRevisionWithDraftMessage()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        ItemDefinition definition = await SeedDefinitionAsync(
            dbContext,
            publish: false,
            campaignId: CampaignId );
        ItemConfigurationAdministrationService service = CreateService( dbContext );

        ItemCatalogApplicationException exception =
            await Assert.ThrowsAsync<ItemCatalogApplicationException>( () =>
                service.CreateAsync(
                    CreateRequest( definition.Id ),
                    CancellationToken.None ) );

        Assert.Contains( "still a draft", exception.Message );
        Assert.Empty( dbContext.ItemConfigurations );
    }

    [Fact]
    public async Task CreateHidesGlobalDraftRevisionAsNotFound()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        ItemDefinition definition = await SeedDefinitionAsync( dbContext, publish: false );
        ItemConfigurationAdministrationService service = CreateService( dbContext );

        ItemCatalogApplicationException exception =
            await Assert.ThrowsAsync<ItemCatalogApplicationException>( () =>
                service.CreateAsync(
                    CreateRequest( definition.Id ),
                    CancellationToken.None ) );

        Assert.Contains( "revision was not found", exception.Message );
        Assert.DoesNotContain( "draft", exception.Message );
        Assert.Empty( dbContext.ItemConfigurations );
    }

    [Fact]
    public async Task CreateReturnsExistingConfigurationWhenCommitLosesRace()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        ItemDefinition definition = await SeedDefinitionAsync( dbContext, publish: true );
        ItemRevision revision = Assert.Single( definition.Revisions );
        RacingUnitOfWork unitOfWork = new RacingUnitOfWork( dbContext, revision.Id );
        ItemConfigurationAdministrationService service = new ItemConfigurationAdministrationService(
            new ItemDefinitionRepository( dbContext ),
            new ItemConfigurationRepository( dbContext ),
            new FakeAdministrativeAccess
            {
                AllowedUserId = GameMasterId,
                AllowedCampaignId = CampaignId,
            },
            unitOfWork,
            new FixedTimeProvider( _now ) );

        ItemConfigurationDto result = await service.CreateAsync(
            CreateRequest( definition.Id ),
            CancellationToken.None );

        Assert.False( result.WasCreated );
        Assert.Equal( unitOfWork.CompetingConfigurationId, result.ItemConfigurationId );
    }

    [Fact]
    public async Task CreateFailsForRetiredRevisionWithRetiredMessage()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        ItemDefinition definition = await SeedDefinitionAsync( dbContext, publish: true );
        definition.RetireRevision( 1, _now.AddMinutes( 5 ) );
        await dbContext.SaveChangesAsync();
        ItemConfigurationAdministrationService service = CreateService( dbContext );

        ItemCatalogApplicationException exception =
            await Assert.ThrowsAsync<ItemCatalogApplicationException>( () =>
                service.CreateAsync(
                    CreateRequest( definition.Id ),
                    CancellationToken.None ) );

        Assert.Contains( "has been retired", exception.Message );
        Assert.Empty( dbContext.ItemConfigurations );
    }

    [Fact]
    public async Task CreateDeniedWithoutGameMasterMembership()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        ItemDefinition definition = await SeedDefinitionAsync( dbContext, publish: true );
        ItemConfigurationAdministrationService service = CreateService(
            dbContext,
            new FakeAdministrativeAccess() );

        await Assert.ThrowsAsync<ItemCatalogAccessDeniedException>( () =>
            service.CreateAsync(
                CreateRequest( definition.Id ),
                CancellationToken.None ) );

        Assert.Empty( dbContext.ItemConfigurations );
    }

    [Fact]
    public async Task CreateFailsForDefinitionOfAnotherCampaign()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        ItemDefinition definition = await SeedDefinitionAsync(
            dbContext,
            publish: true,
            campaignId: CampaignId + 1 );
        ItemConfigurationAdministrationService service = CreateService( dbContext );

        ItemCatalogApplicationException exception =
            await Assert.ThrowsAsync<ItemCatalogApplicationException>( () =>
                service.CreateAsync(
                    CreateRequest( definition.Id ),
                    CancellationToken.None ) );

        Assert.Contains( "was not found in this campaign", exception.Message );
        Assert.Empty( dbContext.ItemConfigurations );
    }

    [Fact]
    public async Task CreateFailsForMissingRevisionNumber()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        ItemDefinition definition = await SeedDefinitionAsync( dbContext, publish: true );
        ItemConfigurationAdministrationService service = CreateService( dbContext );

        ItemCatalogApplicationException exception =
            await Assert.ThrowsAsync<ItemCatalogApplicationException>( () =>
                service.CreateAsync(
                    CreateRequest( definition.Id ) with
                    {
                        RevisionNumber = 99,
                    },
                    CancellationToken.None ) );

        Assert.Contains( "revision was not found", exception.Message );
    }

    private static ItemConfigurationAdministrationService CreateService(
        ItemCatalogDbContext dbContext,
        FakeAdministrativeAccess? access = null )
    {
        return new ItemConfigurationAdministrationService(
            new ItemDefinitionRepository( dbContext ),
            new ItemConfigurationRepository( dbContext ),
            access ?? new FakeAdministrativeAccess
            {
                AllowedUserId = GameMasterId,
                AllowedCampaignId = CampaignId,
            },
            new DbContextUnitOfWork( dbContext ),
            new FixedTimeProvider( _now ) );
    }

    private static CreateItemConfigurationRequest CreateRequest( int itemDefinitionId ) =>
        new CreateItemConfigurationRequest(
            CampaignId,
            itemDefinitionId,
            1,
            ItemSize.Medium,
            ItemMaterialType.Standard,
            ItemMaterialGrade.Standard,
            [],
            GameMasterId );

    private static async Task<ItemDefinition> SeedDefinitionAsync(
        ItemCatalogDbContext dbContext,
        bool publish,
        int? campaignId = null )
    {
        ItemDefinition definition = campaignId is int scopedCampaignId
            ? ItemDefinition.CreateForCampaign(
                "equipment.test-kit",
                scopedCampaignId,
                _now )
            : ItemDefinition.CreateGlobal( "equipment.test-kit", _now );
        definition.CreateRevision(
            "Test kit",
            "Typed test equipment.",
            1,
            10,
            1,
            ItemRevisionRules.Create(
                ItemCategory.OtherEquipment,
                equipment: EquipmentComponent.Create( EquipmentUsage.Held, 1 ) ),
            _now.AddMinutes( 1 ) );
        if ( publish )
        {
            definition.PublishRevision( 1, _now.AddMinutes( 2 ) );
        }

        dbContext.ItemDefinitions.Add( definition );
        await dbContext.SaveChangesAsync();
        return definition;
    }

    private static ItemCatalogDbContext CreateContext()
    {
        DbContextOptions<ItemCatalogDbContext> options =
            new DbContextOptionsBuilder<ItemCatalogDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new ItemCatalogDbContext( options );
    }

    private sealed class FakeAdministrativeAccess : IItemCatalogAdministrativeAccess
    {
        public int? AllowedUserId { get; init; }
        public int? AllowedCampaignId { get; init; }

        public Task<bool> CanManageGlobalCatalogAsync(
            string userName,
            CancellationToken cancellationToken )
        {
            return Task.FromResult( false );
        }

        public Task<bool> CanManageCampaignCatalogAsync(
            int userId,
            int campaignId,
            CancellationToken cancellationToken )
        {
            return Task.FromResult(
                ( AllowedUserId == userId ) &&
                ( AllowedCampaignId == campaignId ) );
        }
    }

    private sealed class DbContextUnitOfWork : IUnitOfWork
    {
        private readonly ItemCatalogDbContext _dbContext;

        public DbContextUnitOfWork( ItemCatalogDbContext dbContext )
        {
            _dbContext = dbContext;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }

        public Task Rollback()
        {
            return Task.CompletedTask;
        }

        public Task BeginTransaction()
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _value;

        public FixedTimeProvider( DateTimeOffset value )
        {
            _value = value;
        }

        public override DateTimeOffset GetUtcNow() => _value;
    }

    private sealed class RacingUnitOfWork : IUnitOfWork
    {
        private readonly ItemCatalogDbContext _dbContext;
        private readonly int _itemRevisionId;

        public RacingUnitOfWork( ItemCatalogDbContext dbContext, int itemRevisionId )
        {
            _dbContext = dbContext;
            _itemRevisionId = itemRevisionId;
        }

        public int CompetingConfigurationId { get; private set; }

        public async Task Commit()
        {
            EntityEntry[] addedEntries = _dbContext.ChangeTracker
                .Entries()
                .Where( entry => entry.State == EntityState.Added )
                .ToArray();
            foreach ( EntityEntry entry in addedEntries )
            {
                entry.State = EntityState.Detached;
            }

            ItemConfiguration competing = ItemConfiguration.Create(
                CampaignId,
                _itemRevisionId,
                ItemSize.Medium,
                ItemMaterialType.Standard,
                ItemMaterialGrade.Standard,
                [],
                _now );
            _dbContext.ItemConfigurations.Add( competing );
            await _dbContext.SaveChangesAsync();
            CompetingConfigurationId = competing.Id;
            throw new InvalidOperationException( "Simulated unique index violation." );
        }

        public Task Rollback()
        {
            return Task.CompletedTask;
        }

        public Task BeginTransaction()
        {
            return Task.CompletedTask;
        }
    }
}
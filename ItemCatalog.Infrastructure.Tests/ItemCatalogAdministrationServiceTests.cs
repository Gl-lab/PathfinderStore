using Microsoft.EntityFrameworkCore;
using Pathfinder.ItemCatalog.Application.Administration;
using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.ItemCatalog.Infrastructure.Items;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.ItemCatalog.Infrastructure.Tests;

public sealed class ItemCatalogAdministrationServiceTests
{
    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 22, 12, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task GlobalDraftRequiresSystemAdministrationPermission()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        FakeAdministrativeAccess access = new FakeAdministrativeAccess();
        ItemCatalogAdministrationService service = CreateService( dbContext, access );

        await Assert.ThrowsAsync<ItemCatalogAccessDeniedException>( () =>
            service.CreateDraftAsync(
                CreateRequest( ItemCatalogScope.Global, null, 7, "admin" ),
                CancellationToken.None ) );

        Assert.Equal( "admin", access.LastGlobalUserName );
        Assert.Empty( dbContext.ItemDefinitions );
    }

    [Fact]
    public async Task CampaignDraftChecksGameMasterAccessForExactCampaign()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        FakeAdministrativeAccess access = new FakeAdministrativeAccess
        {
            AllowedCampaignId = 42,
            AllowedUserId = 7
        };
        ItemCatalogAdministrationService service = CreateService( dbContext, access );

        ItemRevisionDto result = await service.CreateDraftAsync(
            CreateRequest( ItemCatalogScope.Campaign, 42, 7, "gm" ),
            CancellationToken.None );

        Assert.Equal( ItemCatalogScope.Campaign, result.Scope );
        Assert.Equal( 42, result.CampaignId );
        Assert.Equal( (7, 42), access.LastCampaignCheck );
    }

    [Fact]
    public async Task EquivalentKeysRemainIsolatedBetweenCampaigns()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        FakeAdministrativeAccess firstCampaignAccess = new FakeAdministrativeAccess
        {
            AllowedCampaignId = 42,
            AllowedUserId = 7
        };
        FakeAdministrativeAccess secondCampaignAccess = new FakeAdministrativeAccess
        {
            AllowedCampaignId = 43,
            AllowedUserId = 7
        };

        ItemRevisionDto first = await CreateService( dbContext, firstCampaignAccess )
            .CreateDraftAsync(
                CreateRequest( ItemCatalogScope.Campaign, 42, 7, "gm" ),
                CancellationToken.None );
        ItemRevisionDto second = await CreateService( dbContext, secondCampaignAccess )
            .CreateDraftAsync(
                CreateRequest( ItemCatalogScope.Campaign, 43, 7, "gm" ),
                CancellationToken.None );

        Assert.NotEqual( first.ItemDefinitionId, second.ItemDefinitionId );
        Assert.Equal( 2, dbContext.ItemDefinitions.Count() );
        Assert.Contains(
            dbContext.ItemDefinitions,
            definition => definition.CampaignId == 42 );
        Assert.Contains(
            dbContext.ItemDefinitions,
            definition => definition.CampaignId == 43 );
    }

    [Fact]
    public async Task GameMasterCannotPublishAnotherCampaignDraft()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        FakeAdministrativeAccess ownerAccess = new FakeAdministrativeAccess
        {
            AllowedCampaignId = 42,
            AllowedUserId = 7
        };
        ItemRevisionDto draft = await CreateService( dbContext, ownerAccess )
            .CreateDraftAsync(
                CreateRequest( ItemCatalogScope.Campaign, 42, 7, "owner-gm" ),
                CancellationToken.None );
        FakeAdministrativeAccess otherCampaignAccess = new FakeAdministrativeAccess
        {
            AllowedCampaignId = 43,
            AllowedUserId = 8
        };

        await Assert.ThrowsAsync<ItemCatalogAccessDeniedException>( () =>
            CreateService( dbContext, otherCampaignAccess )
                .PublishAsync(
                    draft.ItemDefinitionId,
                    draft.RevisionNumber,
                    8,
                    "other-gm",
                    CancellationToken.None ) );

        Assert.Equal( (8, 42), otherCampaignAccess.LastCampaignCheck );
        Assert.Equal(
            ItemRevisionStatus.Draft,
            Assert.Single( dbContext.ItemDefinitions )
                .Revisions
                .Single()
                .Status );
    }

    [Fact]
    public async Task DraftCanBePublishedAndLifecycleIsPersisted()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        FakeAdministrativeAccess access = new FakeAdministrativeAccess
        {
            CanManageGlobal = true
        };
        ItemCatalogAdministrationService service = CreateService( dbContext, access );
        ItemRevisionDto draft = await service.CreateDraftAsync(
            CreateRequest( ItemCatalogScope.Global, null, 7, "admin" ),
            CancellationToken.None );

        ItemRevisionDto published = await service.PublishAsync(
            draft.ItemDefinitionId,
            draft.RevisionNumber,
            7,
            "admin",
            CancellationToken.None );

        dbContext.ChangeTracker.Clear();
        ItemDefinitionRepository repository = new ItemDefinitionRepository( dbContext );
        ItemDefinition? persisted = await repository.GetByIdWithRevisionsAsync(
            draft.ItemDefinitionId,
            CancellationToken.None );
        Assert.NotNull( persisted );
        Assert.Equal( ItemRevisionStatus.Published, published.Status );
        Assert.Equal(
            ItemRevisionStatus.Published,
            Assert.Single( persisted.Revisions ).Status );
    }

    [Fact]
    public async Task EquivalentTrimmedKeyAddsRevisionToExistingDefinition()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        FakeAdministrativeAccess access = new FakeAdministrativeAccess
        {
            CanManageGlobal = true
        };
        ItemCatalogAdministrationService service = CreateService( dbContext, access );
        await service.CreateDraftAsync(
            CreateRequest( ItemCatalogScope.Global, null, 7, "admin" ),
            CancellationToken.None );
        CreateItemDraftRequest secondRequest = CreateRequest(
            ItemCatalogScope.Global,
            null,
            7,
            "admin" ) with
        {
            Key = "  equipment.test-kit  "
        };

        ItemRevisionDto second = await service.CreateDraftAsync(
            secondRequest,
            CancellationToken.None );

        Assert.Equal( 2, second.RevisionNumber );
        Assert.Single( dbContext.ItemDefinitions );
    }

    [Fact]
    public async Task CreateDraftForDefinitionAppendsNextRevisionNumber()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        FakeAdministrativeAccess access = new FakeAdministrativeAccess
        {
            CanManageGlobal = true
        };
        ItemCatalogAdministrationService service = CreateService( dbContext, access );
        ItemRevisionDto first = await service.CreateDraftAsync(
            CreateRequest( ItemCatalogScope.Global, null, 7, "admin" ),
            CancellationToken.None );

        ItemRevisionDto second = await service.CreateDraftForDefinitionAsync(
            CreateRevisionRequest( first.ItemDefinitionId, 7, "admin" ),
            CancellationToken.None );

        Assert.Equal( first.ItemDefinitionId, second.ItemDefinitionId );
        Assert.Equal( 2, second.RevisionNumber );
        Assert.Equal( ItemRevisionStatus.Draft, second.Status );
        Assert.Equal( "Test kit v2", second.Name );
        Assert.Equal(
            2,
            Assert.Single( dbContext.ItemDefinitions ).Revisions.Count );
    }

    [Fact]
    public async Task CreateDraftForDefinitionDeniedForForeignCampaignGameMaster()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        FakeAdministrativeAccess ownerAccess = new FakeAdministrativeAccess
        {
            AllowedCampaignId = 42,
            AllowedUserId = 7
        };
        ItemRevisionDto draft = await CreateService( dbContext, ownerAccess )
            .CreateDraftAsync(
                CreateRequest( ItemCatalogScope.Campaign, 42, 7, "owner-gm" ),
                CancellationToken.None );
        FakeAdministrativeAccess otherAccess = new FakeAdministrativeAccess
        {
            AllowedCampaignId = 43,
            AllowedUserId = 8
        };

        await Assert.ThrowsAsync<ItemCatalogAccessDeniedException>( () =>
            CreateService( dbContext, otherAccess )
                .CreateDraftForDefinitionAsync(
                    CreateRevisionRequest( draft.ItemDefinitionId, 8, "other-gm" ),
                    CancellationToken.None ) );

        Assert.Single(
            Assert.Single( dbContext.ItemDefinitions ).Revisions );
    }

    [Fact]
    public async Task CreateDraftForDefinitionFailsWhenDefinitionMissing()
    {
        await using ItemCatalogDbContext dbContext = CreateContext();
        ItemCatalogAdministrationService service = CreateService(
            dbContext,
            new FakeAdministrativeAccess
            {
                CanManageGlobal = true
            } );

        ItemCatalogApplicationException exception =
            await Assert.ThrowsAsync<ItemCatalogApplicationException>( () =>
                service.CreateDraftForDefinitionAsync(
                    CreateRevisionRequest( 999, 7, "admin" ),
                    CancellationToken.None ) );

        Assert.Contains( "was not found", exception.Message );
    }

    private static CreateItemRevisionDraftRequest CreateRevisionRequest(
        int itemDefinitionId,
        int actingUserId,
        string actingUserName )
    {
        ItemRevisionRules rules = ItemRevisionRules.Create(
            ItemCategory.OtherEquipment,
            equipment: EquipmentComponent.Create( EquipmentUsage.Held, 1 ) );
        return new CreateItemRevisionDraftRequest(
            itemDefinitionId,
            "Test kit v2",
            "Improved typed test equipment.",
            2,
            20,
            1,
            rules,
            actingUserId,
            actingUserName );
    }

    private static ItemCatalogAdministrationService CreateService(
        ItemCatalogDbContext dbContext,
        FakeAdministrativeAccess access )
    {
        return new ItemCatalogAdministrationService(
            new ItemDefinitionRepository( dbContext ),
            access,
            new DbContextUnitOfWork( dbContext ),
            new FixedTimeProvider( _now ) );
    }

    private static CreateItemDraftRequest CreateRequest(
        ItemCatalogScope scope,
        int? campaignId,
        int actingUserId,
        string actingUserName )
    {
        ItemRevisionRules rules = ItemRevisionRules.Create(
            ItemCategory.OtherEquipment,
            equipment: EquipmentComponent.Create( EquipmentUsage.Held, 1 ) );
        return new CreateItemDraftRequest(
            scope,
            campaignId,
            "equipment.test-kit",
            "Test kit",
            "Typed test equipment.",
            1,
            10,
            1,
            rules,
            actingUserId,
            actingUserName );
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
        public bool CanManageGlobal { get; init; }
        public int? AllowedUserId { get; init; }
        public int? AllowedCampaignId { get; init; }
        public string? LastGlobalUserName { get; private set; }
        public (int UserId, int CampaignId)? LastCampaignCheck { get; private set; }

        public Task<bool> CanManageGlobalCatalogAsync(
            string userName,
            CancellationToken cancellationToken )
        {
            LastGlobalUserName = userName;
            return Task.FromResult( CanManageGlobal );
        }

        public Task<bool> CanManageCampaignCatalogAsync(
            int userId,
            int campaignId,
            CancellationToken cancellationToken )
        {
            LastCampaignCheck = (userId, campaignId);
            return Task.FromResult(
                AllowedUserId == userId &&
                AllowedCampaignId == campaignId );
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
}
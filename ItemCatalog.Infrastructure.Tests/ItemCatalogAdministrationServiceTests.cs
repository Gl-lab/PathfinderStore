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
}
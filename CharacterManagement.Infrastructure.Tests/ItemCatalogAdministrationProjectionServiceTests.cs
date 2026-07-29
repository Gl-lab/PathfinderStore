using Microsoft.EntityFrameworkCore;
using Pathfinder.ItemCatalog.Application.Administration;
using Pathfinder.ItemCatalog.Application.Exceptions;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.Web.Integration;

namespace Pathfinder.CharacterManagement.Infrastructure.Tests;

public sealed class ItemCatalogAdministrationProjectionServiceTests
{
    private const int CampaignId = 42;
    private const int GameMasterId = 7;
    private const int AdminUserId = 99;
    private const string AdminUserName = "admin";
    private const string GameMasterUserName = "gm";

    private static readonly DateTimeOffset _now =
        new DateTimeOffset( 2026, 7, 27, 18, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task AdministratorSeesGlobalDraftsWithoutCampaign()
    {
        await using ItemCatalogDbContext dbContext = CreateCatalogContext();
        await SeedGlobalDefinitionAsync( dbContext, "item.draft-only", "Draft Only", publish: false );
        await SeedGlobalDefinitionAsync( dbContext, "item.published", "Published Item", publish: true );
        ItemCatalogAdministrationProjectionService service = CreateService( dbContext );

        ItemDefinitionAdministrationListDto result = await service.SearchDefinitionsAsync(
            CreateRequest( AdminUserName, campaignId: null ),
            CancellationToken.None );

        Assert.Equal( 2, result.TotalCount );
        ItemDefinitionAdministrationDto draftDefinition = result.Items
            .Single( item => item.Key == "item.draft-only" );
        Assert.Equal(
            ItemRevisionStatus.Draft,
            Assert.Single( draftDefinition.Revisions ).Status );
    }

    [Fact]
    public async Task GameMasterSeesOwnCampaignDraftsAndGlobalPublishedOnly()
    {
        await using ItemCatalogDbContext dbContext = CreateCatalogContext();
        await SeedGlobalDefinitionAsync( dbContext, "item.global-draft", "Global Draft", publish: false );
        await SeedGlobalDefinitionAsync( dbContext, "item.global-published", "Global Published", publish: true );
        await SeedCampaignDefinitionAsync( dbContext, CampaignId, "campaign.own-draft", "Own Draft", publish: false );
        await SeedCampaignDefinitionAsync( dbContext, CampaignId + 1, "campaign.foreign", "Foreign Item", publish: true );
        ItemCatalogAdministrationProjectionService service = CreateService( dbContext );

        ItemDefinitionAdministrationListDto result = await service.SearchDefinitionsAsync(
            CreateRequest( GameMasterUserName, CampaignId ),
            CancellationToken.None );

        string[] keys = result.Items
            .Select( item => item.Key )
            .OrderBy( key => key )
            .ToArray();
        Assert.Equal( [ "campaign.own-draft", "item.global-published" ], keys );
        Assert.Equal( 2, result.TotalCount );
        ItemDefinitionAdministrationDto ownDraft = result.Items
            .Single( item => item.Key == "campaign.own-draft" );
        Assert.Equal(
            ItemRevisionStatus.Draft,
            Assert.Single( ownDraft.Revisions ).Status );
    }

    [Fact]
    public async Task GameMasterDoesNotSeeGlobalDraftRevisionOfMixedDefinition()
    {
        await using ItemCatalogDbContext dbContext = CreateCatalogContext();
        ItemDefinition definition = await SeedGlobalDefinitionAsync(
            dbContext,
            "item.mixed",
            "Mixed Item",
            publish: true );
        definition.CreateRevision(
            "Mixed Item v2",
            "Second revision.",
            2,
            20,
            1,
            CreateRules(),
            _now.AddMinutes( 5 ) );
        await dbContext.SaveChangesAsync();
        ItemCatalogAdministrationProjectionService service = CreateService( dbContext );

        ItemDefinitionAdministrationListDto gmResult = await service.SearchDefinitionsAsync(
            CreateRequest( GameMasterUserName, CampaignId ),
            CancellationToken.None );
        ItemDefinitionAdministrationListDto adminResult = await service.SearchDefinitionsAsync(
            CreateRequest( AdminUserName, campaignId: null ),
            CancellationToken.None );

        ItemDefinitionAdministrationDto gmDefinition = Assert.Single( gmResult.Items );
        Assert.Single( gmDefinition.Revisions );
        Assert.Equal(
            ItemRevisionStatus.Published,
            gmDefinition.Revisions.Single().Status );
        ItemDefinitionAdministrationDto adminDefinition = Assert.Single( adminResult.Items );
        Assert.Equal( 2, adminDefinition.Revisions.Count );
    }

    [Fact]
    public async Task MemberWithoutRolesIsDenied()
    {
        await using ItemCatalogDbContext dbContext = CreateCatalogContext();
        ItemCatalogAdministrationProjectionService service = CreateService( dbContext );

        await Assert.ThrowsAsync<ItemCatalogAccessDeniedException>( () =>
            service.SearchDefinitionsAsync(
                CreateRequest( "member", campaignId: null ),
                CancellationToken.None ) );
        await Assert.ThrowsAsync<ItemCatalogAccessDeniedException>( () =>
            service.SearchDefinitionsAsync(
                CreateRequest( "member", CampaignId + 1 ),
                CancellationToken.None ) );
    }

    [Fact]
    public async Task SearchMatchesKeyAndRevisionName()
    {
        await using ItemCatalogDbContext dbContext = CreateCatalogContext();
        await SeedGlobalDefinitionAsync( dbContext, "item.healing-potion", "Зелье лечения", publish: true );
        await SeedGlobalDefinitionAsync( dbContext, "item.spyglass", "Подзорная труба", publish: true );
        ItemCatalogAdministrationProjectionService service = CreateService( dbContext );

        ItemDefinitionAdministrationListDto byKey = await service.SearchDefinitionsAsync(
            CreateRequest( AdminUserName, null ) with
            {
                Search = "healing",
            },
            CancellationToken.None );
        ItemDefinitionAdministrationListDto byName = await service.SearchDefinitionsAsync(
            CreateRequest( AdminUserName, null ) with
            {
                Search = "  ЗЕЛЬЕ ",
            },
            CancellationToken.None );

        Assert.Equal( "item.healing-potion", Assert.Single( byKey.Items ).Key );
        Assert.Equal( "item.healing-potion", Assert.Single( byName.Items ).Key );
        Assert.Equal( 1, byName.TotalCount );
    }

    [Fact]
    public async Task StatusFilterKeepsDefinitionsWithMatchingRevisions()
    {
        await using ItemCatalogDbContext dbContext = CreateCatalogContext();
        await SeedGlobalDefinitionAsync( dbContext, "item.draft-only", "Draft Only", publish: false );
        await SeedGlobalDefinitionAsync( dbContext, "item.published", "Published Item", publish: true );
        ItemCatalogAdministrationProjectionService service = CreateService( dbContext );

        ItemDefinitionAdministrationListDto drafts = await service.SearchDefinitionsAsync(
            CreateRequest( AdminUserName, null ) with
            {
                Status = ItemRevisionStatus.Draft,
            },
            CancellationToken.None );
        ItemDefinitionAdministrationListDto gmDrafts = await service.SearchDefinitionsAsync(
            CreateRequest( GameMasterUserName, CampaignId ) with
            {
                Status = ItemRevisionStatus.Draft,
            },
            CancellationToken.None );

        Assert.Equal( "item.draft-only", Assert.Single( drafts.Items ).Key );
        Assert.Empty( gmDrafts.Items );
        Assert.Equal( 0, gmDrafts.TotalCount );
    }

    [Fact]
    public async Task ScopeFilterNarrowsResult()
    {
        await using ItemCatalogDbContext dbContext = CreateCatalogContext();
        await SeedGlobalDefinitionAsync( dbContext, "item.global", "Global Item", publish: true );
        await SeedCampaignDefinitionAsync( dbContext, CampaignId, "campaign.item", "Campaign Item", publish: true );
        ItemCatalogAdministrationProjectionService service = CreateService( dbContext );

        ItemDefinitionAdministrationListDto globalOnly = await service.SearchDefinitionsAsync(
            CreateRequest( GameMasterUserName, CampaignId ) with
            {
                Scope = ItemCatalogScopeFilter.Global,
            },
            CancellationToken.None );
        ItemDefinitionAdministrationListDto campaignOnly = await service.SearchDefinitionsAsync(
            CreateRequest( GameMasterUserName, CampaignId ) with
            {
                Scope = ItemCatalogScopeFilter.Campaign,
            },
            CancellationToken.None );

        Assert.Equal( "item.global", Assert.Single( globalOnly.Items ).Key );
        Assert.Equal( "campaign.item", Assert.Single( campaignOnly.Items ).Key );
    }

    [Fact]
    public async Task PagingReturnsTotalCountAndClampsTake()
    {
        await using ItemCatalogDbContext dbContext = CreateCatalogContext();
        for ( int index = 1; index <= 5; index++ )
        {
            await SeedGlobalDefinitionAsync(
                dbContext,
                $"item.paged-{index}",
                $"Paged {index}",
                publish: true );
        }

        ItemCatalogAdministrationProjectionService service = CreateService( dbContext );

        ItemDefinitionAdministrationListDto page = await service.SearchDefinitionsAsync(
            CreateRequest( AdminUserName, null ) with
            {
                Skip = 2,
                Take = 2,
            },
            CancellationToken.None );
        ItemDefinitionAdministrationListDto clamped = await service.SearchDefinitionsAsync(
            CreateRequest( AdminUserName, null ) with
            {
                Skip = 0,
                Take = -10,
            },
            CancellationToken.None );

        Assert.Equal( 5, page.TotalCount );
        Assert.Equal( 2, page.Items.Count );
        Assert.Equal( "item.paged-3", page.Items.First().Key );
        Assert.Equal( 5, clamped.Items.Count );
    }

    [Fact]
    public async Task InvalidScopeFilterFailsWithBusinessError()
    {
        await using ItemCatalogDbContext dbContext = CreateCatalogContext();
        ItemCatalogAdministrationProjectionService service = CreateService( dbContext );

        await Assert.ThrowsAsync<ItemCatalogApplicationException>( () =>
            service.SearchDefinitionsAsync(
                CreateRequest( AdminUserName, null ) with
                {
                    Scope = ( ItemCatalogScopeFilter )7,
                },
                CancellationToken.None ) );
    }

    [Fact]
    public async Task AdministratorWithoutMembershipDoesNotSeeCampaignDefinitions()
    {
        await using ItemCatalogDbContext dbContext = CreateCatalogContext();
        await SeedGlobalDefinitionAsync( dbContext, "item.global-draft", "Global Draft", publish: false );
        await SeedCampaignDefinitionAsync( dbContext, CampaignId, "campaign.item", "Campaign Item", publish: true );
        ItemCatalogAdministrationProjectionService service = CreateService( dbContext );

        ItemDefinitionAdministrationListDto result = await service.SearchDefinitionsAsync(
            CreateRequest( AdminUserName, CampaignId ) with
            {
                ActingUserId = AdminUserId,
            },
            CancellationToken.None );

        Assert.Equal( "item.global-draft", Assert.Single( result.Items ).Key );
        Assert.Equal( 1, result.TotalCount );
    }

    private static ItemCatalogAdministrationProjectionService CreateService(
        ItemCatalogDbContext dbContext )
    {
        return new ItemCatalogAdministrationProjectionService(
            dbContext,
            new StubAdministrativeAccess() );
    }

    private static ItemCatalogAdministrationSearchRequest CreateRequest(
        string actingUserName,
        int? campaignId )
    {
        return new ItemCatalogAdministrationSearchRequest(
            ItemCatalogScopeFilter.All,
            campaignId,
            null,
            null,
            0,
            50,
            GameMasterId,
            actingUserName );
    }

    private static async Task<ItemDefinition> SeedGlobalDefinitionAsync(
        ItemCatalogDbContext dbContext,
        string key,
        string name,
        bool publish )
    {
        ItemDefinition definition = ItemDefinition.CreateGlobal( key, _now );
        AddRevision( definition, name, publish );
        dbContext.ItemDefinitions.Add( definition );
        await dbContext.SaveChangesAsync();
        return definition;
    }

    private static async Task<ItemDefinition> SeedCampaignDefinitionAsync(
        ItemCatalogDbContext dbContext,
        int campaignId,
        string key,
        string name,
        bool publish )
    {
        ItemDefinition definition = ItemDefinition.CreateForCampaign( key, campaignId, _now );
        AddRevision( definition, name, publish );
        dbContext.ItemDefinitions.Add( definition );
        await dbContext.SaveChangesAsync();
        return definition;
    }

    private static void AddRevision( ItemDefinition definition, string name, bool publish )
    {
        definition.CreateRevision(
            name,
            $"{name} description.",
            1,
            100,
            1,
            CreateRules(),
            _now.AddMinutes( 1 ) );
        if ( publish )
        {
            definition.PublishRevision( 1, _now.AddMinutes( 2 ) );
        }
    }

    private static ItemRevisionRules CreateRules() =>
        ItemRevisionRules.Create(
            ItemCategory.OtherEquipment,
            equipment: EquipmentComponent.Create( EquipmentUsage.Held, 1 ) );

    private static ItemCatalogDbContext CreateCatalogContext()
    {
        DbContextOptions<ItemCatalogDbContext> options =
            new DbContextOptionsBuilder<ItemCatalogDbContext>()
                .UseInMemoryDatabase( Guid.NewGuid().ToString() )
                .Options;
        return new ItemCatalogDbContext( options );
    }

    private sealed class StubAdministrativeAccess : IItemCatalogAdministrativeAccess
    {
        public Task<bool> CanManageGlobalCatalogAsync(
            string userName,
            CancellationToken cancellationToken )
        {
            return Task.FromResult( userName == AdminUserName );
        }

        public Task<bool> CanManageCampaignCatalogAsync(
            int userId,
            int campaignId,
            CancellationToken cancellationToken )
        {
            return Task.FromResult(
                ( userId == GameMasterId ) &&
                ( campaignId == CampaignId ) );
        }
    }
}
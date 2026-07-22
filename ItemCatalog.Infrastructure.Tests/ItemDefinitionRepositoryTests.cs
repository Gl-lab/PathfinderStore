using Microsoft.EntityFrameworkCore;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Infrastructure.Data;
using Pathfinder.ItemCatalog.Infrastructure.Items;

namespace Pathfinder.ItemCatalog.Infrastructure.Tests;

public sealed class ItemDefinitionRepositoryTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 10, 0, 0, TimeSpan.Zero );

    [Fact]
    public async Task RepositoryPersistsDefinitionAndImmutableRevisionSnapshots()
    {
        DbContextOptions<ItemCatalogDbContext> options = CreateOptions();
        await using ( ItemCatalogDbContext writeContext = new ItemCatalogDbContext( options ) )
        {
            ItemDefinition definition = ItemDefinition.Create(
                "equipment.longsword",
                _createdAtUtc );
            definition.CreateRevision(
                "Longsword",
                "Original rules text.",
                0,
                100,
                1,
                _createdAtUtc.AddMinutes( 1 ) );
            writeContext.ItemDefinitions.Add( definition );
            await writeContext.SaveChangesAsync();
        }

        await using ( ItemCatalogDbContext readContext = new ItemCatalogDbContext( options ) )
        {
            ItemDefinitionRepository repository = new ItemDefinitionRepository( readContext );
            ItemDefinition? definition = await repository.GetByKeyWithRevisionsAsync(
                "equipment.longsword",
                CancellationToken.None );

            Assert.NotNull( definition );
            ItemRevision revision = Assert.Single( definition.Revisions );
            Assert.Equal( 1, revision.RevisionNumber );
            Assert.Equal( "Original rules text.", revision.Description );

            definition.CreateRevision(
                "Longsword",
                "Replacement rules text.",
                1,
                120,
                1,
                _createdAtUtc.AddMinutes( 2 ) );
            await readContext.SaveChangesAsync();
        }

        await using ( ItemCatalogDbContext verifyContext = new ItemCatalogDbContext( options ) )
        {
            ItemDefinition definition = await verifyContext.ItemDefinitions
                .Include( item => item.Revisions )
                .SingleAsync();
            ItemRevision[] revisions = definition.Revisions
                .OrderBy( revision => revision.RevisionNumber )
                .ToArray();

            Assert.Equal( 2, revisions.Length );
            Assert.Equal( "Original rules text.", revisions[ 0 ].Description );
            Assert.Equal( "Replacement rules text.", revisions[ 1 ].Description );
        }
    }

    private static DbContextOptions<ItemCatalogDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ItemCatalogDbContext>()
            .UseInMemoryDatabase( Guid.NewGuid().ToString() )
            .Options;
    }
}
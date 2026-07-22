using Microsoft.EntityFrameworkCore;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;
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
                CreateLongswordRules(),
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
                CreateLongswordRules(),
                _createdAtUtc.AddMinutes( 2 ) );
            await readContext.SaveChangesAsync();
        }

        await using ( ItemCatalogDbContext verifyContext = new ItemCatalogDbContext( options ) )
        {
            ItemDefinitionRepository repository = new ItemDefinitionRepository( verifyContext );
            ItemDefinition? definition = await repository.GetByKeyWithRevisionsAsync(
                "equipment.longsword",
                CancellationToken.None );
            Assert.NotNull( definition );
            ItemRevision[] revisions = definition.Revisions
                .OrderBy( revision => revision.RevisionNumber )
                .ToArray();

            Assert.Equal( 2, revisions.Length );
            Assert.Equal( "Original rules text.", revisions[ 0 ].Description );
            Assert.Equal( "Replacement rules text.", revisions[ 1 ].Description );
            Assert.Equal( ItemCategory.Weapon, revisions[ 0 ].PrimaryCategory );
            Assert.Single( revisions[ 0 ].Attacks );
            Assert.NotNull( revisions[ 0 ].Equipment );
            Assert.NotNull( revisions[ 0 ].Durability );
        }
    }

    [Fact]
    public async Task RepositoryRehydratesCompositeTypedComponents()
    {
        DbContextOptions<ItemCatalogDbContext> options = CreateOptions();
        await using ( ItemCatalogDbContext writeContext = new ItemCatalogDbContext( options ) )
        {
            ItemDefinition definition = ItemDefinition.Create( "custom.arcane-buckler", _createdAtUtc );
            ItemRevisionRules rules = ItemRevisionRules.Create(
                ItemCategory.Shield,
                attacks:
                [
                    AttackComponent.Create(
                        "Shield bash",
                        1,
                        DamageDieSize.D4,
                        ItemDamageType.Bludgeoning,
                        1 ),
                ],
                armor: ArmorComponent.Create( ArmorCategory.Light, 1, 4, -1, 0, 1 ),
                shield: ShieldComponent.Create( 2 ),
                equipment: EquipmentComponent.Create( EquipmentUsage.Held, 1 ),
                consumption: ConsumptionComponent.Create( ConsumptionMode.ReduceStack, 1 ),
                charges: ChargeComponent.Create( 3, 1, ChargeRecoveryRule.DailyPreparations ),
                durability: DurabilityComponent.Create( 5, 20, 10 ) );
            definition.CreateRevision(
                "Arcane buckler",
                "Composite rules fixture.",
                2,
                300,
                1,
                rules,
                _createdAtUtc.AddMinutes( 1 ) );
            writeContext.ItemDefinitions.Add( definition );
            await writeContext.SaveChangesAsync();
        }

        await using ( ItemCatalogDbContext readContext = new ItemCatalogDbContext( options ) )
        {
            ItemDefinitionRepository repository = new ItemDefinitionRepository( readContext );
            ItemDefinition? definition = await repository.GetByKeyWithRevisionsAsync(
                "custom.arcane-buckler",
                CancellationToken.None );

            Assert.NotNull( definition );
            ItemRevision revision = Assert.Single( definition.Revisions );
            Assert.Single( revision.Attacks );
            Assert.NotNull( revision.Armor );
            Assert.NotNull( revision.Shield );
            Assert.NotNull( revision.Equipment );
            Assert.NotNull( revision.Consumption );
            Assert.NotNull( revision.Charges );
            Assert.NotNull( revision.Durability );
        }
    }

    private static ItemRevisionRules CreateLongswordRules()
    {
        return ItemRevisionRules.Create(
            ItemCategory.Weapon,
            attacks:
            [
                AttackComponent.Create(
                    "Blade",
                    1,
                    DamageDieSize.D8,
                    ItemDamageType.Slashing,
                    1 ),
            ],
            equipment: EquipmentComponent.Create( EquipmentUsage.Held, 1 ),
            durability: DurabilityComponent.Create( 5, 20, 10 ) );
    }

    private static DbContextOptions<ItemCatalogDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ItemCatalogDbContext>()
            .UseInMemoryDatabase( Guid.NewGuid().ToString() )
            .Options;
    }
}
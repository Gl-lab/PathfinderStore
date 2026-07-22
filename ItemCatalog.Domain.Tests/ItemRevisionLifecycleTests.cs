using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;

namespace Pathfinder.ItemCatalog.Domain.Tests;

public sealed class ItemRevisionLifecycleTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 12, 0, 0, TimeSpan.Zero );

    [Fact]
    public void PublishTransitionsDraftToPublished()
    {
        ItemDefinition definition = CreateDefinition();

        ItemRevision revision = definition.PublishRevision( 1, _createdAtUtc.AddMinutes( 1 ) );

        Assert.Equal( ItemRevisionStatus.Published, revision.Status );
        Assert.Equal( _createdAtUtc.AddMinutes( 1 ), revision.PublishedAtUtc );
        Assert.Null( revision.RetiredAtUtc );
    }

    [Fact]
    public void PublishingNextRevisionRetiresPreviousSnapshot()
    {
        ItemDefinition definition = CreateDefinition();
        ItemRevision first = definition.PublishRevision( 1, _createdAtUtc.AddMinutes( 1 ) );
        ItemRevision second = definition.CreateRevision(
            "Longsword remastered",
            String.Empty,
            1,
            120,
            1,
            CreateRules(),
            _createdAtUtc.AddMinutes( 2 ) );

        definition.PublishRevision( 2, _createdAtUtc.AddMinutes( 3 ) );

        Assert.Equal( ItemRevisionStatus.Retired, first.Status );
        Assert.Equal( _createdAtUtc.AddMinutes( 3 ), first.RetiredAtUtc );
        Assert.Equal( ItemRevisionStatus.Published, second.Status );
        Assert.Equal( "Longsword", first.Name );
    }

    [Fact]
    public void RetireRejectsDraftRevision()
    {
        ItemDefinition definition = CreateDefinition();

        Assert.Throws<ItemCatalogException>( () =>
            definition.RetireRevision( 1, _createdAtUtc.AddMinutes( 1 ) ) );
    }

    [Fact]
    public void PublishedRevisionCannotBePublishedAgain()
    {
        ItemDefinition definition = CreateDefinition();
        definition.PublishRevision( 1, _createdAtUtc.AddMinutes( 1 ) );

        Assert.Throws<ItemCatalogException>( () =>
            definition.PublishRevision( 1, _createdAtUtc.AddMinutes( 2 ) ) );
    }

    private static ItemDefinition CreateDefinition()
    {
        ItemDefinition definition = ItemDefinition.CreateGlobal(
            "equipment.longsword",
            _createdAtUtc );
        definition.CreateRevision(
            "Longsword",
            String.Empty,
            0,
            100,
            1,
            CreateRules(),
            _createdAtUtc );
        return definition;
    }

    private static ItemRevisionRules CreateRules()
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
}

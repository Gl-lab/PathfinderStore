using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.ItemCatalog.Domain.Items;
using Pathfinder.ItemCatalog.Domain.Rules;

namespace Pathfinder.ItemCatalog.Domain.Tests;

public sealed class ItemDefinitionTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 10, 0, 0, TimeSpan.Zero );

    [Fact]
    public void CreateKeepsStableLogicalKey()
    {
        ItemDefinition definition = ItemDefinition.CreateGlobal(
            "  equipment.longsword  ",
            _createdAtUtc );

        Assert.Equal( "equipment.longsword", definition.Key );
        Assert.Equal( ItemCatalogScope.Global, definition.Scope );
        Assert.Null( definition.CampaignId );
        Assert.Equal( _createdAtUtc, definition.CreatedAtUtc );
        Assert.Empty( definition.Revisions );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( "Equipment.Longsword" )]
    [InlineData( "equipment..longsword" )]
    [InlineData( "equipment.long sword" )]
    [InlineData( "equipment.long_sword" )]
    public void CreateRejectsInvalidLogicalKey( string key )
    {
        Assert.Throws<ItemCatalogException>( () => ItemDefinition.CreateGlobal( key, _createdAtUtc ) );
    }

    [Fact]
    public void CreateRevisionAssignsConsecutiveSnapshotNumbers()
    {
        ItemDefinition definition = ItemDefinition.CreateGlobal( "equipment.longsword", _createdAtUtc );

        ItemRevision firstRevision = definition.CreateRevision(
            " Longsword ",
            " Versatile martial blade. ",
            0,
            100,
            1,
            CreateBasicRules(),
            _createdAtUtc.AddMinutes( 1 ) );
        ItemRevision secondRevision = definition.CreateRevision(
            "Longsword (remastered)",
            "Updated rules text.",
            1,
            120,
            1,
            CreateBasicRules(),
            _createdAtUtc.AddMinutes( 2 ) );

        Assert.Equal( 1, firstRevision.RevisionNumber );
        Assert.Equal( "Longsword", firstRevision.Name );
        Assert.Equal( "Versatile martial blade.", firstRevision.Description );
        Assert.Equal( 0, firstRevision.Level );
        Assert.Equal( 100, firstRevision.PriceInCopperPieces );
        Assert.Equal( 1, firstRevision.Bulk );
        Assert.Equal( 2, secondRevision.RevisionNumber );
        Assert.Equal( "Longsword (remastered)", secondRevision.Name );
        Assert.Equal( 2, definition.Revisions.Count );
    }

    [Fact]
    public void LaterRevisionDoesNotChangePreviousSnapshot()
    {
        ItemDefinition definition = ItemDefinition.CreateGlobal( "equipment.longsword", _createdAtUtc );
        ItemRevision firstRevision = definition.CreateRevision(
            "Longsword",
            "Original rules text.",
            0,
            100,
            1,
            CreateBasicRules(),
            _createdAtUtc.AddMinutes( 1 ) );

        definition.CreateRevision(
            "Longsword",
            "Replacement rules text.",
            1,
            120,
            1,
            CreateBasicRules(),
            _createdAtUtc.AddMinutes( 2 ) );

        Assert.Equal( "Original rules text.", firstRevision.Description );
        Assert.Equal( 0, firstRevision.Level );
        Assert.Equal( 100, firstRevision.PriceInCopperPieces );
    }

    [Fact]
    public void RevisionCollectionCannotBeChangedByCaller()
    {
        ItemDefinition definition = ItemDefinition.CreateGlobal( "equipment.longsword", _createdAtUtc );
        definition.CreateRevision(
            "Longsword",
            String.Empty,
            0,
            100,
            1,
            CreateBasicRules(),
            _createdAtUtc.AddMinutes( 1 ) );
        IList<ItemRevision> revisions = Assert.IsAssignableFrom<IList<ItemRevision>>(
            definition.Revisions );

        Assert.True( revisions.IsReadOnly );
        Assert.Throws<NotSupportedException>( () => revisions.Clear() );
    }

    [Theory]
    [InlineData( -1, 100, 1 )]
    [InlineData( 0, -1, 1 )]
    [InlineData( 0, 100, -1 )]
    public void CreateRevisionRejectsInvalidNumericSnapshot(
        int level,
        int priceInCopperPieces,
        decimal bulk )
    {
        ItemDefinition definition = ItemDefinition.CreateGlobal( "equipment.longsword", _createdAtUtc );

        Assert.Throws<ItemCatalogException>( () => definition.CreateRevision(
            "Longsword",
            String.Empty,
            level,
            priceInCopperPieces,
            bulk,
            CreateBasicRules(),
            _createdAtUtc.AddMinutes( 1 ) ) );
    }

    [Fact]
    public void CreateRevisionRejectsTimestampBeforePreviousRevision()
    {
        ItemDefinition definition = ItemDefinition.CreateGlobal( "equipment.longsword", _createdAtUtc );
        definition.CreateRevision(
            "Longsword",
            String.Empty,
            0,
            100,
            1,
            CreateBasicRules(),
            _createdAtUtc.AddMinutes( 2 ) );

        Assert.Throws<ItemCatalogException>( () => definition.CreateRevision(
            "Longsword",
            String.Empty,
            0,
            100,
            1,
            CreateBasicRules(),
            _createdAtUtc.AddMinutes( 1 ) ) );
    }

    [Fact]
    public void CreateRevisionRejectsRulesComponentSetAlreadyOwnedByAnotherRevision()
    {
        ItemDefinition definition = ItemDefinition.CreateGlobal( "equipment.longsword", _createdAtUtc );
        ItemRevisionRules rules = CreateBasicRules();
        definition.CreateRevision(
            "Longsword",
            String.Empty,
            0,
            100,
            1,
            rules,
            _createdAtUtc.AddMinutes( 1 ) );

        Assert.Throws<ItemCatalogException>( () => definition.CreateRevision(
            "Longsword",
            String.Empty,
            1,
            120,
            1,
            rules,
            _createdAtUtc.AddMinutes( 2 ) ) );
    }

    [Fact]
    public void CreateForCampaignKeepsCampaignScope()
    {
        ItemDefinition definition = ItemDefinition.CreateForCampaign(
            "custom.sun-blade",
            42,
            _createdAtUtc );

        Assert.Equal( ItemCatalogScope.Campaign, definition.Scope );
        Assert.Equal( 42, definition.CampaignId );
    }

    [Fact]
    public void CreateForCampaignRejectsInvalidCampaignId()
    {
        Assert.Throws<ItemCatalogException>( () => ItemDefinition.CreateForCampaign(
            "custom.sun-blade",
            0,
            _createdAtUtc ) );
    }

    private static ItemRevisionRules CreateBasicRules()
    {
        return ItemRevisionRules.Create(
            ItemCategory.OtherEquipment,
            equipment: EquipmentComponent.Create( EquipmentUsage.Worn, 0 ) );
    }
}
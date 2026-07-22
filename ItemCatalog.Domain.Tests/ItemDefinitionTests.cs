using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.ItemCatalog.Domain.Items;

namespace Pathfinder.ItemCatalog.Domain.Tests;

public sealed class ItemDefinitionTests
{
    private static readonly DateTimeOffset _createdAtUtc =
        new DateTimeOffset( 2026, 7, 22, 10, 0, 0, TimeSpan.Zero );

    [Fact]
    public void CreateKeepsStableLogicalKey()
    {
        ItemDefinition definition = ItemDefinition.Create(
            "  equipment.longsword  ",
            _createdAtUtc );

        Assert.Equal( "equipment.longsword", definition.Key );
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
        Assert.Throws<ItemCatalogException>( () => ItemDefinition.Create( key, _createdAtUtc ) );
    }

    [Fact]
    public void CreateRevisionAssignsConsecutiveSnapshotNumbers()
    {
        ItemDefinition definition = ItemDefinition.Create( "equipment.longsword", _createdAtUtc );

        ItemRevision firstRevision = definition.CreateRevision(
            " Longsword ",
            " Versatile martial blade. ",
            0,
            100,
            1,
            _createdAtUtc.AddMinutes( 1 ) );
        ItemRevision secondRevision = definition.CreateRevision(
            "Longsword (remastered)",
            "Updated rules text.",
            1,
            120,
            1,
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
        ItemDefinition definition = ItemDefinition.Create( "equipment.longsword", _createdAtUtc );
        ItemRevision firstRevision = definition.CreateRevision(
            "Longsword",
            "Original rules text.",
            0,
            100,
            1,
            _createdAtUtc.AddMinutes( 1 ) );

        definition.CreateRevision(
            "Longsword",
            "Replacement rules text.",
            1,
            120,
            1,
            _createdAtUtc.AddMinutes( 2 ) );

        Assert.Equal( "Original rules text.", firstRevision.Description );
        Assert.Equal( 0, firstRevision.Level );
        Assert.Equal( 100, firstRevision.PriceInCopperPieces );
    }

    [Fact]
    public void RevisionCollectionCannotBeChangedByCaller()
    {
        ItemDefinition definition = ItemDefinition.Create( "equipment.longsword", _createdAtUtc );
        definition.CreateRevision(
            "Longsword",
            String.Empty,
            0,
            100,
            1,
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
        ItemDefinition definition = ItemDefinition.Create( "equipment.longsword", _createdAtUtc );

        Assert.Throws<ItemCatalogException>( () => definition.CreateRevision(
            "Longsword",
            String.Empty,
            level,
            priceInCopperPieces,
            bulk,
            _createdAtUtc.AddMinutes( 1 ) ) );
    }

    [Fact]
    public void CreateRevisionRejectsTimestampBeforePreviousRevision()
    {
        ItemDefinition definition = ItemDefinition.Create( "equipment.longsword", _createdAtUtc );
        definition.CreateRevision(
            "Longsword",
            String.Empty,
            0,
            100,
            1,
            _createdAtUtc.AddMinutes( 2 ) );

        Assert.Throws<ItemCatalogException>( () => definition.CreateRevision(
            "Longsword",
            String.Empty,
            0,
            100,
            1,
            _createdAtUtc.AddMinutes( 1 ) ) );
    }
}
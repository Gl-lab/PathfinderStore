using Pathfinder.CharacterManagement.Application.Avatars;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class AvatarSelectorTests
{
    [Fact]
    public void NotSpecifiedAlwaysUsesUnknownWithoutReadingCatalog()
    {
        ThrowingAvatarCatalog catalog = new ThrowingAvatarCatalog();
        AvatarSelector selector = new AvatarSelector( catalog, new FixedIndexProvider( 0 ) );

        AvatarId result = selector.Select( new AvatarSelectionCriteria(
            AncestryType.Human,
            "class.fighter",
            CharacterGender.NotSpecified ) );

        Assert.Equal( AvatarIds.Unknown, result );
        Assert.False( catalog.WasRead );
    }

    [Fact]
    public void ExactPoolUsesIndexProvider()
    {
        AvatarDefinition first = CreateAvatar( "avatar.human.fighter.female.01" );
        AvatarDefinition second = CreateAvatar( "avatar.human.fighter.female.02" );
        StubAvatarCatalog catalog = new StubAvatarCatalog( [ first, second ] );
        AvatarSelector selector = new AvatarSelector( catalog, new FixedIndexProvider( 1 ) );

        AvatarId result = selector.Select( new AvatarSelectionCriteria(
            AncestryType.Human,
            "class.fighter",
            CharacterGender.Female ) );

        Assert.Equal( second.Id, result );
    }

    [Fact]
    public void EmptyPoolUsesUnknown()
    {
        AvatarSelector selector = new AvatarSelector(
            new StubAvatarCatalog( [] ),
            new FixedIndexProvider( 0 ) );

        AvatarId result = selector.Select( new AvatarSelectionCriteria(
            AncestryType.Human,
            "class.fighter",
            CharacterGender.Male ) );

        Assert.Equal( AvatarIds.Unknown, result );
    }

    [Fact]
    public void CatalogFailureUsesUnknown()
    {
        AvatarSelector selector = new AvatarSelector(
            new ThrowingAvatarCatalog(),
            new FixedIndexProvider( 0 ) );

        AvatarId result = selector.Select( new AvatarSelectionCriteria(
            AncestryType.Human,
            "class.fighter",
            CharacterGender.Male ) );

        Assert.Equal( AvatarIds.Unknown, result );
    }

    [Fact]
    public void CatalogPrefersMostSpecificCompatiblePool()
    {
        AvatarDefinition genericAvatar = CreateAvatar( "avatar.human.fighter.female.01" );
        AvatarDefinition heritageAvatar = genericAvatar with
        {
            Id = AvatarId.Create( "avatar.human.fighter.female.skilled.01" ),
            HeritageId = "human.skilled"
        };
        AvatarCatalog catalog = new AvatarCatalog( [ genericAvatar, heritageAvatar ] );

        IReadOnlyList<AvatarDefinition> result = catalog.FindMatches( new AvatarSelectionCriteria(
            AncestryType.Human,
            "class.fighter",
            CharacterGender.Female,
            HeritageId: "human.skilled" ) );

        Assert.Equal( heritageAvatar, Assert.Single( result ) );
    }

    [Theory]
    [InlineData( "class.bard", CharacterGender.Male, 2 )]
    [InlineData( "class.bard", CharacterGender.Female, 2 )]
    [InlineData( "class.cleric", CharacterGender.Male, 2 )]
    [InlineData( "class.cleric", CharacterGender.Female, 2 )]
    [InlineData( "class.druid", CharacterGender.Male, 2 )]
    public void RuntimeCatalogReturnsOnlyAcceptedDwarfAssets(
        string characterClassId,
        CharacterGender gender,
        int expectedCount )
    {
        AvatarCatalog catalog = new AvatarCatalog();

        IReadOnlyList<AvatarDefinition> result = catalog.FindMatches( new AvatarSelectionCriteria(
            AncestryType.Dwarf,
            characterClassId,
            gender ) );

        Assert.Equal( expectedCount, result.Count );
        Assert.All( result, avatar => Assert.NotNull( avatar.Variant ) );
        Assert.All( result, avatar => Assert.StartsWith( "/avatars/pc/", avatar.Path ) );
        Assert.All( result, avatar => Assert.EndsWith( ".webp", avatar.Path ) );
    }

    [Fact]
    public void RuntimeCatalogLeavesUncoveredCombinationEmpty()
    {
        AvatarCatalog catalog = new AvatarCatalog();

        IReadOnlyList<AvatarDefinition> result = catalog.FindMatches( new AvatarSelectionCriteria(
            AncestryType.Dwarf,
            "class.druid",
            CharacterGender.Female ) );

        Assert.Empty( result );
    }

    private static AvatarDefinition CreateAvatar( string id ) => new AvatarDefinition(
        AvatarId.Create( id ),
        $"/avatars/{id}.webp",
        AncestryType.Human,
        "class.fighter",
        CharacterGender.Female );

    private sealed class FixedIndexProvider( int index ) : IAvatarSelectionIndexProvider
    {
        public int Next( int exclusiveUpperBound ) => index;
    }

    private sealed class StubAvatarCatalog( IReadOnlyList<AvatarDefinition> matches ) : IAvatarCatalog
    {
        public IReadOnlyList<AvatarDefinition> FindMatches( AvatarSelectionCriteria criteria ) => matches;

        public string ResolvePath( AvatarId avatarId ) => AvatarCatalog.UnknownPath;
    }

    private sealed class ThrowingAvatarCatalog : IAvatarCatalog
    {
        public bool WasRead { get; private set; }

        public IReadOnlyList<AvatarDefinition> FindMatches( AvatarSelectionCriteria criteria )
        {
            WasRead = true;
            throw new InvalidOperationException( "Catalog is unavailable." );
        }

        public string ResolvePath( AvatarId avatarId ) => throw new InvalidOperationException();
    }
}

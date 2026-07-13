using Pathfinder.CharacterManagement.Application.Builders.Implementation;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public sealed class ClassPackageTests
{
    [Fact]
    public void SetClassPackage_ValidChoice_AppliesClassBoost()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass fighter = CreateClass(
            "class.fighter",
            AbilityType.Strength,
            AbilityType.Dexterity );

        character.SetClassPackage( fighter, AbilityType.Strength );

        Assert.Equal( fighter.Id, character.SelectedClassId );
        Assert.Equal( AbilityType.Strength, character.SelectedClassKeyAbility );
        Assert.Equal( 12, character.AbilityScores.Strength.Value );
        Assert.True( character.HasClassBoostPackage );
    }

    [Fact]
    public void SetClassPackage_BackgroundNotSet_ThrowsWithoutChangingCharacter()
    {
        DraftCharacter character = CreateCharacterWithoutBackground();
        CharacterClass fighter = CreateClass( "class.fighter", AbilityType.Strength );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage( fighter, AbilityType.Strength ) );

        Assert.Null( character.SelectedClassId );
        Assert.Equal( 10, character.AbilityScores.Strength.Value );
    }

    [Fact]
    public void SetClassPackage_KeyAbilityNotAllowed_ThrowsWithoutChangingCharacter()
    {
        DraftCharacter character = CreateCharacter();
        CharacterClass bard = CreateClass( "class.bard", AbilityType.Charisma );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetClassPackage( bard, AbilityType.Strength ) );

        Assert.Null( character.SelectedClassId );
        Assert.Equal( 10, character.AbilityScores.Strength.Value );
    }

    [Fact]
    public void SetClassPackage_KeyAbilityAlreadyBoostedByPreviousPackages_IsAllowed()
    {
        DraftCharacter character = CreateCharacter();
        Background background = CreateBackground();
        CharacterClass wizard = CreateClass( "class.wizard", AbilityType.Intelligence );
        character.SetBackgroundPackage(
            background,
            AbilityType.Dexterity,
            AbilityType.Intelligence );

        character.SetClassPackage( wizard, AbilityType.Intelligence );

        Assert.Equal( 16, character.AbilityScores.Intelligence.Value );
    }

    [Fact]
    public void SetClassPackage_CalledTwice_ReplacesOnlyClassBoost()
    {
        DraftCharacter character = CreateCharacter();
        Background background = CreateBackground();
        CharacterClass wizard = CreateClass( "class.wizard", AbilityType.Intelligence );
        CharacterClass bard = CreateClass( "class.bard", AbilityType.Charisma );
        character.SetBackgroundPackage(
            background,
            AbilityType.Dexterity,
            AbilityType.Intelligence );
        character.SetClassPackage( wizard, AbilityType.Intelligence );

        character.SetClassPackage( bard, AbilityType.Charisma );

        Assert.Equal( bard.Id, character.SelectedClassId );
        Assert.Equal( 14, character.AbilityScores.Intelligence.Value );
        Assert.Equal( 12, character.AbilityScores.Charisma.Value );
        Assert.Equal( 12, character.AbilityScores.Dexterity.Value );
    }

    [Fact]
    public void CharacterBuilder_SetClass_UsesCatalogEntry()
    {
        Ancestry ancestry = CreateAncestry();
        CharacterClass fighter = CreateClass(
            "class.fighter",
            AbilityType.Strength,
            AbilityType.Dexterity );
        CharacterBuilder builder = new CharacterBuilder(
            new StubAncestryRepository( ancestry ),
            backgroundRepository: new StubBackgroundRepository( CreateBackground() ),
            characterClassRepository: new StubCharacterClassRepository( fighter ) );
        builder.CreateCharacter( 1, "Tester", AncestryType.Human );
        builder.SetAncestry( AncestryType.Human );
        builder.ApplyFreeBoosts( [ AbilityType.Intelligence, AbilityType.Wisdom ] );
        builder.SetBackground(
            "background.acrobat",
            AbilityType.Dexterity,
            AbilityType.Constitution );

        builder.SetClass( fighter.Id, AbilityType.Dexterity );

        DraftCharacter character = builder.Build();
        Assert.Equal( fighter.Id, character.SelectedClassId );
        Assert.Equal( 14, character.AbilityScores.Dexterity.Value );
    }

    private static DraftCharacter CreateCharacter()
    {
        DraftCharacter character = CreateCharacterWithoutBackground();
        character.SetBackgroundPackage(
            CreateBackground(),
            AbilityType.Dexterity,
            AbilityType.Constitution );
        return character;
    }

    private static DraftCharacter CreateCharacterWithoutBackground()
    {
        Ancestry ancestry = CreateAncestry();
        DraftCharacter character = DraftCharacter.Create( 1, "Tester", AncestryType.Human );
        character.SetAncestry( ancestry );
        character.SetFreeBoosts( [ AbilityType.Intelligence, AbilityType.Wisdom ] );
        return character;
    }

    private static Ancestry CreateAncestry()
    {
        return new Ancestry(
            AncestryType.Human,
            [ AncestryBoostSlot.Free(), AncestryBoostSlot.Free() ],
            [],
            8,
            RaceSizeType.Medium,
            25 );
    }

    private static Background CreateBackground()
    {
        return new Background(
            "background.acrobat",
            "Acrobat",
            new SourceReference( "Player Core", 60 ),
            [ AbilityType.Strength, AbilityType.Dexterity ],
            1,
            [] );
    }

    private static CharacterClass CreateClass( string id, params AbilityType[] keyAbilities )
    {
        return new CharacterClass(
            id,
            id,
            new SourceReference( "Player Core", 1 ),
            8,
            keyAbilities,
            null,
            [],
            [] );
    }

    private sealed class StubAncestryRepository : IAncestryRepository
    {
        private readonly Ancestry _ancestry;

        public StubAncestryRepository( Ancestry ancestry )
        {
            _ancestry = ancestry;
        }

        public IReadOnlyCollection<Ancestry> GetAll() => [ _ancestry ];

        public Ancestry GetAncestry( AncestryType ancestryType ) => _ancestry;
    }

    private sealed class StubCharacterClassRepository : ICharacterClassRepository
    {
        private readonly CharacterClass _characterClass;

        public StubCharacterClassRepository( CharacterClass characterClass )
        {
            _characterClass = characterClass;
        }

        public IReadOnlyCollection<CharacterClass> GetAll() => [ _characterClass ];

        public CharacterClass GetCharacterClass( string characterClassId ) => _characterClass;
    }

    private sealed class StubBackgroundRepository : IBackgroundRepository
    {
        private readonly Background _background;

        public StubBackgroundRepository( Background background )
        {
            _background = background;
        }

        public IReadOnlyCollection<Background> GetAll() => [ _background ];

        public Background GetBackground( string backgroundId ) => _background;
    }
}

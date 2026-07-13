using Pathfinder.CharacterManagement.Application.Builders.Implementation;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public sealed class BackgroundPackageTests
{
    [Fact]
    public void SetBackgroundPackage_ValidChoices_AppliesBothBoosts()
    {
        DraftCharacter character = CreateHumanCharacter();
        Background background = CreateBackground(
            "background.acrobat",
            AbilityType.Strength,
            AbilityType.Dexterity );

        character.SetBackgroundPackage(
            background,
            AbilityType.Dexterity,
            AbilityType.Intelligence );

        Assert.Equal( background.Id, character.SelectedBackgroundId );
        Assert.Equal( AbilityType.Dexterity, character.SelectedBackgroundRestrictedBoost );
        Assert.Equal( AbilityType.Intelligence, character.SelectedBackgroundFreeBoost );
        Assert.Equal( 12, character.AbilityScores.Dexterity.Value );
        Assert.Equal( 14, character.AbilityScores.Intelligence.Value );
        Assert.True( character.HasBackgroundBoostPackage );
    }

    [Fact]
    public void SetBackgroundPackage_RestrictedBoostNotInOptions_ThrowsWithoutChangingCharacter()
    {
        DraftCharacter character = CreateHumanCharacter();
        Background background = CreateBackground(
            "background.acrobat",
            AbilityType.Strength,
            AbilityType.Dexterity );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetBackgroundPackage(
                background,
                AbilityType.Wisdom,
                AbilityType.Charisma ) );

        Assert.Null( character.SelectedBackgroundId );
        Assert.Equal( 12, character.AbilityScores.Wisdom.Value );
        Assert.Equal( 10, character.AbilityScores.Charisma.Value );
    }

    [Fact]
    public void SetBackgroundPackage_DuplicateBoosts_Throws()
    {
        DraftCharacter character = CreateHumanCharacter();
        Background background = CreateBackground(
            "background.acrobat",
            AbilityType.Strength,
            AbilityType.Dexterity );

        Assert.Throws<CharacterManagementException>( () =>
            character.SetBackgroundPackage(
                background,
                AbilityType.Dexterity,
                AbilityType.Dexterity ) );
    }

    [Fact]
    public void SetBackgroundPackage_BoostAlreadyAppliedByAncestry_IsAllowed()
    {
        DraftCharacter character = CreateDwarfCharacter();
        Background background = CreateBackground(
            "background.field_medic",
            AbilityType.Constitution,
            AbilityType.Wisdom );

        character.SetBackgroundPackage(
            background,
            AbilityType.Constitution,
            AbilityType.Intelligence );

        Assert.Equal( 14, character.AbilityScores.Constitution.Value );
        Assert.Equal( 14, character.AbilityScores.Intelligence.Value );
    }

    [Fact]
    public void SetBackgroundPackage_CalledTwice_ReplacesOnlyBackgroundBoosts()
    {
        DraftCharacter character = CreateDwarfCharacter();
        Background firstBackground = CreateBackground(
            "background.field_medic",
            AbilityType.Constitution,
            AbilityType.Wisdom );
        Background nextBackground = CreateBackground(
            "background.acrobat",
            AbilityType.Strength,
            AbilityType.Dexterity );

        character.SetBackgroundPackage(
            firstBackground,
            AbilityType.Constitution,
            AbilityType.Intelligence );
        character.SetBackgroundPackage(
            nextBackground,
            AbilityType.Dexterity,
            AbilityType.Charisma );

        Assert.Equal( nextBackground.Id, character.SelectedBackgroundId );
        Assert.Equal( 12, character.AbilityScores.Constitution.Value );
        Assert.Equal( 12, character.AbilityScores.Intelligence.Value );
        Assert.Equal( 12, character.AbilityScores.Dexterity.Value );
        Assert.Equal( 10, character.AbilityScores.Charisma.Value );
    }

    [Fact]
    public void CharacterBuilder_SetBackground_UsesCatalogEntry()
    {
        Ancestry ancestry = CreateHumanAncestry();
        Background background = CreateBackground(
            "background.acrobat",
            AbilityType.Strength,
            AbilityType.Dexterity );
        CharacterBuilder builder = new CharacterBuilder(
            new StubAncestryRepository( ancestry ),
            backgroundRepository: new StubBackgroundRepository( background ) );
        builder.CreateCharacter( 1, "Tester", AncestryType.Human );
        builder.SetAncestry( AncestryType.Human );
        builder.ApplyFreeBoosts( [ AbilityType.Intelligence, AbilityType.Wisdom ] );

        builder.SetBackground(
            background.Id,
            AbilityType.Strength,
            AbilityType.Charisma );

        DraftCharacter character = builder.Build();
        Assert.Equal( background.Id, character.SelectedBackgroundId );
        Assert.Equal( 12, character.AbilityScores.Strength.Value );
        Assert.Equal( 12, character.AbilityScores.Charisma.Value );
    }

    private static DraftCharacter CreateHumanCharacter()
    {
        Ancestry ancestry = CreateHumanAncestry();
        DraftCharacter character = DraftCharacter.Create( 1, "Tester", AncestryType.Human );
        character.SetAncestry( ancestry );
        character.SetFreeBoosts( [ AbilityType.Intelligence, AbilityType.Wisdom ] );
        return character;
    }

    private static DraftCharacter CreateDwarfCharacter()
    {
        Ancestry ancestry = new Ancestry(
            AncestryType.Dwarf,
            [
                AncestryBoostSlot.Fixed( AbilityType.Constitution ),
                AncestryBoostSlot.Fixed( AbilityType.Wisdom ),
                AncestryBoostSlot.Free()
            ],
            [ AbilityType.Charisma ],
            10,
            RaceSizeType.Medium,
            20 );
        DraftCharacter character = DraftCharacter.Create( 1, "Tester", AncestryType.Dwarf );
        character.SetAncestry( ancestry );
        character.SetFreeBoosts( [ AbilityType.Intelligence ] );
        return character;
    }

    private static Ancestry CreateHumanAncestry()
    {
        return new Ancestry(
            AncestryType.Human,
            [ AncestryBoostSlot.Free(), AncestryBoostSlot.Free() ],
            [],
            8,
            RaceSizeType.Medium,
            25 );
    }

    private static Background CreateBackground(
        string id,
        AbilityType firstOption,
        AbilityType secondOption )
    {
        return new Background(
            id,
            id,
            new SourceReference( "Player Core", 60 ),
            [ firstOption, secondOption ],
            1,
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

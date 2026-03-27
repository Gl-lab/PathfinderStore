using Pathfinder.CharacterManagement.Application.Builders.Implementation;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace CharacterManagement.Domain.Tests;

public class CharacterBuilderFreeBoostsTests
{
    private static CharacterBuilder CreateBuilder( Ancestry ancestry )
    {
        IAncestryRepository repository = new StubAncestryRepository( ancestry );
        CharacterBuilder builder = new CharacterBuilder( repository );
        builder.CreateCharacter( accountId: 1, name: "Tester", ancestryType: ancestry.AncestryType );
        builder.SetAncestry( ancestry.AncestryType );
        return builder;
    }

    private static Ancestry HumanAncestry() => new Ancestry(
        AncestryType.Human,
        abilityBoosts: [ AncestryBoostSlot.Free(), AncestryBoostSlot.Free() ],
        abilityFlaws: [],
        baseHitPoints: 8,
        size: RaceSizeType.Medium,
        baseSpeed: 25 );

    private static Ancestry DwarfAncestry() => new Ancestry(
        AncestryType.Dwarf,
        abilityBoosts:
        [
            AncestryBoostSlot.Fixed( AbilityType.Constitution ),
            AncestryBoostSlot.Fixed( AbilityType.Wisdom ),
            AncestryBoostSlot.Free()
        ],
        abilityFlaws: [ AbilityType.Charisma ],
        baseHitPoints: 10,
        size: RaceSizeType.Medium,
        baseSpeed: 20 );

    private static Ancestry ElfAncestry() => new Ancestry(
        AncestryType.Elf,
        abilityBoosts:
        [
            AncestryBoostSlot.Fixed( AbilityType.Dexterity ),
            AncestryBoostSlot.Fixed( AbilityType.Intelligence ),
            AncestryBoostSlot.Free()
        ],
        abilityFlaws: [ AbilityType.Constitution ],
        baseHitPoints: 6,
        size: RaceSizeType.Medium,
        baseSpeed: 30,
        lowLightVision: true );

    [Fact]
    public void ApplyFreeBoosts_CorrectCount_AppliesBoosts()
    {
        CharacterBuilder builder = CreateBuilder( HumanAncestry() );

        builder.ApplyFreeBoosts( [ AbilityType.Strength, AbilityType.Intelligence ] );
        DraftCharacter character = builder.Build();

        Assert.Equal( 12, character.AbilityScores.Strength.Value );
        Assert.Equal( 12, character.AbilityScores.Intelligence.Value );
    }

    [Fact]
    public void ApplyFreeBoosts_CalledTwice_SecondCallOverrides()
    {
        CharacterBuilder builder = CreateBuilder( HumanAncestry() );

        builder.ApplyFreeBoosts( [ AbilityType.Strength, AbilityType.Intelligence ] );
        builder.ApplyFreeBoosts( [ AbilityType.Dexterity, AbilityType.Wisdom ] );
        DraftCharacter character = builder.Build();

        Assert.Equal( 10, character.AbilityScores.Strength.Value );
        Assert.Equal( 10, character.AbilityScores.Intelligence.Value );
        Assert.Equal( 12, character.AbilityScores.Dexterity.Value );
        Assert.Equal( 12, character.AbilityScores.Wisdom.Value );
    }

    [Fact]
    public void ApplyFreeBoosts_WrongCount_Throws()
    {
        CharacterBuilder builder = CreateBuilder( HumanAncestry() );

        Assert.Throws<CharacterManagementException>( () =>
            builder.ApplyFreeBoosts( [ AbilityType.Strength ] ) );
    }

    [Fact]
    public void ApplyFreeBoosts_Duplicate_Throws()
    {
        CharacterBuilder builder = CreateBuilder( HumanAncestry() );

        Assert.Throws<CharacterManagementException>( () =>
            builder.ApplyFreeBoosts( [ AbilityType.Strength, AbilityType.Strength ] ) );
    }

    [Fact]
    public void ApplyFreeBoosts_ConflictsWithFixedBoost_Throws()
    {
        CharacterBuilder builder = CreateBuilder( DwarfAncestry() );

        Assert.Throws<CharacterManagementException>( () =>
            builder.ApplyFreeBoosts( [ AbilityType.Constitution ] ) );
    }

    [Fact]
    public void ApplyFreeBoosts_BeforeSetAncestry_Throws()
    {
        IAncestryRepository repository = new StubAncestryRepository( HumanAncestry() );
        CharacterBuilder builder = new CharacterBuilder( repository );
        builder.CreateCharacter( accountId: 1, name: "Tester", ancestryType: AncestryType.Human );

        Assert.Throws<CharacterManagementException>( () =>
            builder.ApplyFreeBoosts( [ AbilityType.Strength, AbilityType.Intelligence ] ) );
    }

    [Fact]
    public void ApplyFreeBoosts_OneFreeSlot_Ok()
    {
        CharacterBuilder builder = CreateBuilder( DwarfAncestry() );

        builder.ApplyFreeBoosts( [ AbilityType.Intelligence ] );
        DraftCharacter character = builder.Build();

        Assert.Equal( 12, character.AbilityScores.Intelligence.Value );
    }

    [Fact]
    public void ApplyFreeBoosts_EmptyList_WhenZeroFreeSlots_Ok()
    {
        // Раса без свободных слотов
        Ancestry noFreeSlots = new Ancestry(
            AncestryType.Goblin,
            abilityBoosts:
            [
                AncestryBoostSlot.Fixed( AbilityType.Dexterity ),
                AncestryBoostSlot.Fixed( AbilityType.Charisma )
            ],
            abilityFlaws: [ AbilityType.Wisdom ],
            baseHitPoints: 6,
            size: RaceSizeType.Small,
            baseSpeed: 25 );

        CharacterBuilder builder = CreateBuilder( noFreeSlots );

        builder.ApplyFreeBoosts( [] ); // не должно бросать
        DraftCharacter character = builder.Build();

        Assert.Equal( 10, character.AbilityScores.Strength.Value );
    }

    [Fact]
    public void ApplyFreeBoosts_AfterAncestryChange_UsesNewAncestrySlots()
    {
        IAncestryRepository repository = new MultiAncestryRepository( HumanAncestry(), DwarfAncestry() );
        CharacterBuilder builder = new CharacterBuilder( repository );
        builder.CreateCharacter( accountId: 1, name: "Tester", ancestryType: AncestryType.Human );

        builder.SetAncestry( AncestryType.Human );   // 2 free слота
        builder.ApplyFreeBoosts( [ AbilityType.Strength, AbilityType.Intelligence ] );

        builder.SetAncestry( AncestryType.Dwarf );   // 1 free слот
        builder.ApplyFreeBoosts( [ AbilityType.Intelligence ] );

        DraftCharacter character = builder.Build();

        // Human бусты откатились, Dwarf применён
        Assert.Equal( 10, character.AbilityScores.Strength.Value );
        Assert.Equal( 12, character.AbilityScores.Constitution.Value ); // Dwarf Fixed
        Assert.Equal( 12, character.AbilityScores.Intelligence.Value ); // Dwarf Free
    }

    private sealed class StubAncestryRepository : IAncestryRepository
    {
        private readonly Ancestry _ancestry;
        public StubAncestryRepository( Ancestry ancestry )
        {
            _ancestry = ancestry;
        }

        public IReadOnlyCollection<Ancestry> GetAll() => throw new NotImplementedException();

        public Ancestry GetAncestry( AncestryType ancestryType ) => _ancestry;
    }

    private sealed class MultiAncestryRepository : IAncestryRepository
    {
        private readonly Dictionary<AncestryType, Ancestry> _ancestries;

        public MultiAncestryRepository( params Ancestry[] ancestries )
        {
            _ancestries = ancestries.ToDictionary( a => a.AncestryType );
        }

        public IReadOnlyCollection<Ancestry> GetAll() => throw new NotImplementedException();

        public Ancestry GetAncestry( AncestryType ancestryType ) => _ancestries[ ancestryType ];
    }
}

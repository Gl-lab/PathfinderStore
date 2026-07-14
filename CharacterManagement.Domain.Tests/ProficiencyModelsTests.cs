using Pathfinder.CharacterManagement.Domain.Entity;

namespace CharacterManagement.Domain.Tests;

public sealed class ProficiencyModelsTests
{
    [Fact]
    public void Resolve_MultipleSources_ReturnsOneHighestRankPerTarget()
    {
        IReadOnlyList<EffectiveProficiency> result = ProficiencyResolver.Resolve(
        [
            new ProficiencyGrant( ProficiencyTargets.MediumArmor, ProficiencyRank.Trained, "class.source" ),
            new ProficiencyGrant( ProficiencyTargets.MediumArmor, ProficiencyRank.Expert, "racket.source" ),
            new ProficiencyGrant( ProficiencyTargets.Perception, ProficiencyRank.Trained, "class.source" ),
        ] );

        EffectiveProficiency armor = Assert.Single( result.Where( item =>
            item.Target.Id == ProficiencyTargets.MediumArmor.Id ) );
        Assert.Equal( ProficiencyRank.Expert, armor.Rank );
        Assert.Equal( [ "class.source", "racket.source" ], armor.SourceGrantIds );
        Assert.Equal( 2, result.Count );
    }

    [Fact]
    public void Ranks_HaveExpectedProgressionOrder()
    {
        Assert.True( ProficiencyRank.Untrained < ProficiencyRank.Trained );
        Assert.True( ProficiencyRank.Trained < ProficiencyRank.Expert );
        Assert.True( ProficiencyRank.Expert < ProficiencyRank.Master );
        Assert.True( ProficiencyRank.Master < ProficiencyRank.Legendary );
    }

    [Fact]
    public void ClassDc_CreatesStableClassSpecificTarget()
    {
        ProficiencyTarget result = ProficiencyTargets.ClassDc( "class.fighter", "Fighter" );

        Assert.Equal( "proficiency.class_dc.fighter", result.Id );
        Assert.Equal( "Fighter Class DC", result.Name );
        Assert.Equal( ProficiencyCategory.ClassDc, result.Category );
    }

    [Fact]
    public void Grant_UntrainedRank_Throws()
    {
        Assert.Throws<ArgumentException>( () =>
            new ProficiencyGrant(
                ProficiencyTargets.Perception,
                ProficiencyRank.Untrained,
                "class.bard.initial_proficiencies" ) );
    }

    [Fact]
    public void Target_UnknownCategory_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>( () =>
            new ProficiencyTarget(
                "proficiency.unknown",
                "Unknown",
                ( ProficiencyCategory )999 ) );
    }

    [Fact]
    public void ClassDc_EmptyClassSuffix_Throws()
    {
        Assert.Throws<ArgumentException>( () =>
            ProficiencyTargets.ClassDc( "class.", "Unknown" ) );
    }

    [Fact]
    public void CharacterClass_CopiesInitialProficiencies()
    {
        List<ProficiencyGrant> grants =
        [
            new ProficiencyGrant(
                ProficiencyTargets.Perception,
                ProficiencyRank.Trained,
                "class.test.initial_proficiencies" ),
        ];
        CharacterClass characterClass = new CharacterClass(
            "class.test",
            "Test",
            SourceReference.Unknown,
            8,
            [ AbilityType.Strength ],
            grants,
            [],
            0,
            null,
            [],
            [] );

        grants.Clear();

        Assert.Single( characterClass.InitialProficiencies );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( "perception" )]
    public void Target_InvalidId_Throws( string id )
    {
        Assert.Throws<ArgumentException>( () =>
            new ProficiencyTarget(
                id,
                "Perception",
                ProficiencyCategory.Perception ) );
    }
}

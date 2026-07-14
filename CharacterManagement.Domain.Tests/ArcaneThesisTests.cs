using Pathfinder.CharacterManagement.Domain.Entity;

namespace CharacterManagement.Domain.Tests;

public sealed class ArcaneThesisTests
{
    [Fact]
    public void Constructor_ValidThesis_PreservesTypedEffects()
    {
        ArcaneThesisEffectDescriptor effect = CreateEffect(
            ArcaneThesisEffectKind.FamiliarAbilityProgression,
            [ 1, 6, 12, 18 ] );

        ArcaneThesis thesis = new ArcaneThesis(
            "arcane_thesis.test",
            "Test Thesis",
            SourceReference.Unknown,
            [ effect ] );

        Assert.Equal( "arcane_thesis.test", thesis.Id );
        Assert.Equal( [ 1, 6, 12, 18 ], Assert.Single( thesis.Effects ).MilestoneLevels );
    }

    [Theory]
    [InlineData( 0, 1 )]
    [InlineData( 6, 1 )]
    [InlineData( 1, 1 )]
    public void EffectConstructor_InvalidMilestones_Throws( int first, int second )
    {
        Assert.Throws<ArgumentException>( () => CreateEffect(
            ArcaneThesisEffectKind.FamiliarAbilityProgression,
            [ first, second ] ) );
    }

    [Fact]
    public void Constructor_EffectBelongsToAnotherThesis_Throws()
    {
        ArcaneThesisEffectDescriptor effect = new ArcaneThesisEffectDescriptor(
            "arcane_thesis.other.effect.test",
            ArcaneThesisEffectKind.SpellSlotBlending,
            "Test",
            "Test effect.",
            [ 1 ],
            [ CharacterClassDependencyType.SpellSlotRules ] );

        Assert.Throws<ArgumentException>( () => new ArcaneThesis(
            "arcane_thesis.test",
            "Test Thesis",
            SourceReference.Unknown,
            [ effect ] ) );
    }

    [Fact]
    public void EffectConstructor_WithoutDeferredDependencies_Throws()
    {
        Assert.Throws<ArgumentException>( () => new ArcaneThesisEffectDescriptor(
            "arcane_thesis.test.effect.test",
            ArcaneThesisEffectKind.SpellSlotBlending,
            "Test",
            "Test effect.",
            [ 1 ],
            [] ) );
    }

    [Fact]
    public void EffectConstructor_UnknownKind_ThrowsArgumentOutOfRangeException()
    {
        ArcaneThesisEffectKind unknownKind = ( ArcaneThesisEffectKind )Int32.MaxValue;

        Assert.Throws<ArgumentOutOfRangeException>( () => new ArcaneThesisEffectDescriptor(
            "invalid",
            unknownKind,
            "Unknown",
            "Unknown effect.",
            [ 1 ],
            [ CharacterClassDependencyType.ClassFeatureRules ] ) );
    }

    private static ArcaneThesisEffectDescriptor CreateEffect(
        ArcaneThesisEffectKind kind,
        IReadOnlyList<int> milestoneLevels )
    {
        return new ArcaneThesisEffectDescriptor(
            "arcane_thesis.test.effect.test",
            kind,
            "Test",
            "Test effect.",
            milestoneLevels,
            [ CharacterClassDependencyType.FamiliarRules ] );
    }
}

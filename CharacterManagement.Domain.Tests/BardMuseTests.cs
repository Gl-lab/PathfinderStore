using Pathfinder.CharacterManagement.Domain.Entity;

namespace CharacterManagement.Domain.Tests;

public sealed class BardMuseTests
{
    [Fact]
    public void Constructor_ValidMuse_PreservesTypedBenefits()
    {
        BardMuse bardMuse = CreateMuse();

        Assert.Contains(
            bardMuse.Benefits,
            benefit => benefit.Kind == BardMuseBenefitKind.ClassFeat );
        Assert.Contains(
            bardMuse.Benefits,
            benefit => benefit.Kind == BardMuseBenefitKind.RepertoireSpell );
    }

    [Fact]
    public void Constructor_DuplicateBenefitKind_Throws()
    {
        Assert.Throws<ArgumentException>( () => new BardMuse(
            "bard_muse.enigma",
            "Enigma",
            SourceReference.Unknown,
            [
                CreateBenefit( "feat.bardic_lore", BardMuseBenefitKind.ClassFeat ),
                CreateBenefit( "feat.versatile_performance", BardMuseBenefitKind.ClassFeat ),
            ] ) );
    }

    private static BardMuse CreateMuse()
    {
        return new BardMuse(
            "bard_muse.enigma",
            "Enigma",
            SourceReference.Unknown,
            [
                CreateBenefit( "feat.bardic_lore", BardMuseBenefitKind.ClassFeat ),
                CreateBenefit( "spell.sure_strike", BardMuseBenefitKind.RepertoireSpell ),
            ] );
    }

    private static BardMuseBenefitDescriptor CreateBenefit(
        string id,
        BardMuseBenefitKind kind )
    {
        return new BardMuseBenefitDescriptor(
            id,
            kind,
            id,
            [ CharacterClassDependencyType.ClassFeatureRules ] );
    }
}

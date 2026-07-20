using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace CharacterManagement.Domain.Tests;

public sealed class BardSpellLoadoutResolverTests
{
    [Fact]
    public void Resolve_CreatesCompleteRepertoireWithMuseSpell()
    {
        BardMuse muse = CreateMuse( "spell.muse" );
        IReadOnlyCollection<SpellDefinition> catalog = CreateCatalog();

        BardSpellLoadout result = BardSpellLoadoutResolver.Resolve(
            muse,
            [ "spell.cantrip_1", "spell.cantrip_2", "spell.cantrip_3", "spell.cantrip_4", "spell.cantrip_5" ],
            [ "spell.selected_1", "spell.selected_2" ],
            catalog );

        Assert.Equal( 5, result.CantripIds.Count );
        Assert.Equal( [ "spell.selected_1", "spell.selected_2" ], result.SelectedRankOneSpellIds );
        Assert.Equal( "spell.muse", result.MuseGrantedSpellId );
    }

    [Fact]
    public void Resolve_RejectsDuplicateCantrips()
    {
        Assert.Throws<ArgumentException>( () => BardSpellLoadoutResolver.Resolve(
            CreateMuse( "spell.muse" ),
            [ "spell.cantrip_1", "spell.cantrip_1", "spell.cantrip_3", "spell.cantrip_4", "spell.cantrip_5" ],
            [ "spell.selected_1", "spell.selected_2" ],
            CreateCatalog() ) );
    }

    [Fact]
    public void Resolve_RejectsMuseSpellAsSelectedSpell()
    {
        Assert.Throws<ArgumentException>( () => BardSpellLoadoutResolver.Resolve(
            CreateMuse( "spell.muse" ),
            [ "spell.cantrip_1", "spell.cantrip_2", "spell.cantrip_3", "spell.cantrip_4", "spell.cantrip_5" ],
            [ "spell.selected_1", "spell.muse" ],
            CreateCatalog() ) );
    }

    [Fact]
    public void Resolve_RejectsSpellOutsideCommonOccultList()
    {
        Assert.Throws<ArgumentException>( () => BardSpellLoadoutResolver.Resolve(
            CreateMuse( "spell.muse" ),
            [ "spell.cantrip_1", "spell.cantrip_2", "spell.cantrip_3", "spell.cantrip_4", "spell.cantrip_5" ],
            [ "spell.selected_1", "spell.arcane" ],
            CreateCatalog() ) );
    }

    private static BardMuse CreateMuse( string spellId )
    {
        return new BardMuse(
            "bard_muse.test",
            "Test",
            new SourceReference( "Player Core", 1 ),
            [
                new BardMuseBenefitDescriptor(
                    "feat.test",
                    BardMuseBenefitKind.ClassFeat,
                    "Test Feat",
                    [] ),
                new BardMuseBenefitDescriptor(
                    spellId,
                    BardMuseBenefitKind.RepertoireSpell,
                    "Muse Spell",
                    [] ),
            ] );
    }

    private static IReadOnlyCollection<SpellDefinition> CreateCatalog()
    {
        List<SpellDefinition> spells = Enumerable
            .Range( 1, 5 )
            .Select( index => Spell(
                $"spell.cantrip_{index}",
                $"Cantrip {index}",
                SpellKind.Cantrip,
                SpellTradition.Occult ) )
            .ToList();
        spells.Add( Spell( "spell.selected_1", "Selected 1", SpellKind.Spell, SpellTradition.Occult ) );
        spells.Add( Spell( "spell.selected_2", "Selected 2", SpellKind.Spell, SpellTradition.Occult ) );
        spells.Add( Spell( "spell.muse", "Muse", SpellKind.Spell, SpellTradition.Occult ) );
        spells.Add( Spell( "spell.arcane", "Arcane", SpellKind.Spell, SpellTradition.Arcane ) );
        return spells;
    }

    private static SpellDefinition Spell(
        string id,
        string name,
        SpellKind kind,
        SpellTradition tradition )
    {
        return new SpellDefinition(
            id,
            name,
            1,
            kind,
            [ tradition ],
            [],
            SpellRarity.Common,
            new SourceReference( "Player Core", 1 ) );
    }
}

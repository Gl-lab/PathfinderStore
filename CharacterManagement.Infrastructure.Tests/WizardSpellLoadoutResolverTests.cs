using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace Pathfinder.CharacterManagement.Infrastructure.Tests;

public sealed class WizardSpellLoadoutResolverTests
{
    private readonly ArcaneSchoolRepository _schoolRepository = new ArcaneSchoolRepository();
    private readonly SpellRepository _spellRepository = new SpellRepository();

    [Fact]
    public void Resolve_CreatesFormalSchoolSpellbookAndSlots()
    {
        ArcaneSchool school = _schoolRepository.GetArcaneSchool( "arcane_school.battle_magic" );
        SpellDefinition[] cantrips = ArcaneCantrips().Take( 10 ).ToArray();
        SpellDefinition[] spells = ArcaneSpells().Take( 5 ).ToArray();

        WizardSpellLoadout result = WizardSpellLoadoutResolver.Resolve(
            school,
            cantrips.Select( spell => spell.Id ).ToArray(),
            spells.Select( spell => spell.Id ).ToArray(),
            "spell.shield",
            [ "spell.breathe_fire", "spell.force_barrage" ],
            cantrips.Take( 5 ).Select( spell => spell.Id ).ToArray(),
            [ spells[ 0 ].Id, spells[ 0 ].Id ],
            "spell.telekinetic_projectile",
            "spell.mystic_armor",
            _spellRepository.GetAll() );

        Assert.Equal( 10, result.SpellbookCantripIds.Count );
        Assert.Equal( 2, result.CurriculumRankOneSpellIds.Count );
        Assert.Equal( "spell.force_bolt", result.InitialSchoolSpellId );
        Assert.Equal( 1, result.MaximumFocusPoints );
        Assert.Equal( 1, result.DrainBondedItemUsesPerDay );
    }

    [Fact]
    public void Resolve_UsesSixBaseSpellsAndNoCurriculumForUnifiedTheory()
    {
        ArcaneSchool school = _schoolRepository.GetArcaneSchool(
            "arcane_school.unified_magical_theory" );
        SpellDefinition[] cantrips = ArcaneCantrips().Take( 10 ).ToArray();
        SpellDefinition[] spells = ArcaneSpells().Take( 6 ).ToArray();

        WizardSpellLoadout result = WizardSpellLoadoutResolver.Resolve(
            school,
            cantrips.Select( spell => spell.Id ).ToArray(),
            spells.Select( spell => spell.Id ).ToArray(),
            null,
            [],
            cantrips.Take( 5 ).Select( spell => spell.Id ).ToArray(),
            [ spells[ 0 ].Id, spells[ 1 ].Id ],
            null,
            null,
            _spellRepository.GetAll() );

        Assert.Empty( result.CurriculumRankOneSpellIds );
        Assert.Equal( "spell.hand_of_the_apprentice", result.InitialSchoolSpellId );
    }

    [Fact]
    public void Resolve_RejectsSchoolSlotOutsideCurriculum()
    {
        ArcaneSchool school = _schoolRepository.GetArcaneSchool( "arcane_school.battle_magic" );
        SpellDefinition[] cantrips = ArcaneCantrips().Take( 10 ).ToArray();
        SpellDefinition[] spells = ArcaneSpells().Take( 5 ).ToArray();

        Assert.Throws<ArgumentException>( () => WizardSpellLoadoutResolver.Resolve(
            school,
            cantrips.Select( spell => spell.Id ).ToArray(),
            spells.Select( spell => spell.Id ).ToArray(),
            "spell.shield",
            [ "spell.breathe_fire", "spell.force_barrage" ],
            cantrips.Take( 5 ).Select( spell => spell.Id ).ToArray(),
            [ spells[ 0 ].Id, spells[ 1 ].Id ],
            "spell.shield",
            "spell.fear",
            _spellRepository.GetAll() ) );
    }

    private IEnumerable<SpellDefinition> ArcaneCantrips() => _spellRepository
        .GetAll()
        .Where( spell =>
            ( spell.Kind == SpellKind.Cantrip ) &&
            ( spell.Rarity == SpellRarity.Common ) &&
            spell.Traditions.Contains( SpellTradition.Arcane ) );

    private IEnumerable<SpellDefinition> ArcaneSpells() => _spellRepository
        .GetAll()
        .Where( spell =>
            ( spell.Kind == SpellKind.Spell ) &&
            ( spell.Rank == 1 ) &&
            ( spell.Rarity == SpellRarity.Common ) &&
            spell.Traditions.Contains( SpellTradition.Arcane ) );
}

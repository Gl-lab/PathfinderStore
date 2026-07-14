using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace CharacterManagement.Domain.Tests;

public sealed class ClericSpellAvailabilityResolverTests
{
    [Fact]
    public void ResolveCantrips_ReturnsOnlyCommonFirstRankDivineCantrips()
    {
        IReadOnlyCollection<SpellDefinition> catalog =
        [
            Create( "guidance", SpellKind.Cantrip, SpellRarity.Common, [ SpellTradition.Divine ] ),
            Create( "rare_guidance", SpellKind.Cantrip, SpellRarity.Rare, [ SpellTradition.Divine ] ),
            Create( "arcane_cantrip", SpellKind.Cantrip, SpellRarity.Common, [ SpellTradition.Arcane ] ),
            Create( "heal", SpellKind.Spell, SpellRarity.Common, [ SpellTradition.Divine ] ),
        ];

        IReadOnlyList<ClericAvailableSpell> result = ClericSpellAvailabilityResolver.ResolveCantrips( catalog );

        ClericAvailableSpell guidance = Assert.Single( result );
        Assert.Equal( "spell.guidance", guidance.Spell.Id );
        Assert.Equal( SpellTradition.Divine, guidance.EffectiveTradition );
        Assert.Equal( [ ClericSpellAccessSource.DivineTradition ], guidance.AccessSources );
    }

    [Fact]
    public void ResolveRankOneSpells_AddsOnlySelectedDeityGrantAsEffectiveDivineSpell()
    {
        SpellDefinition heal = Create( "heal", SpellKind.Spell, SpellRarity.Common, [ SpellTradition.Divine ] );
        SpellDefinition sureStrike = Create( "sure_strike", SpellKind.Spell, SpellRarity.Common, [ SpellTradition.Arcane ] );
        SpellDefinition fireball = Create( "fireball", SpellKind.Spell, SpellRarity.Common, [ SpellTradition.Arcane ] );
        IReadOnlyCollection<SpellDefinition> catalog = [ heal, sureStrike, fireball ];
        Deity iomedae = CreateDeity( "iomedae", [ new DeityGrantedSpell( 1, sureStrike.Id, sureStrike.Name ) ] );
        Deity pharasma = CreateDeity( "pharasma", [] );

        IReadOnlyList<ClericAvailableSpell> iomedaeSpells =
            ClericSpellAvailabilityResolver.ResolveRankOneSpells( iomedae, catalog );
        IReadOnlyList<ClericAvailableSpell> pharasmaSpells =
            ClericSpellAvailabilityResolver.ResolveRankOneSpells( pharasma, catalog );

        ClericAvailableSpell grantedSpell = Assert.Single(
            iomedaeSpells,
            spell => spell.Spell.Id == sureStrike.Id );
        Assert.Equal( SpellTradition.Divine, grantedSpell.EffectiveTradition );
        Assert.Equal( [ ClericSpellAccessSource.DeityGranted ], grantedSpell.AccessSources );
        Assert.DoesNotContain( pharasmaSpells, spell => spell.Spell.Id == sureStrike.Id );
        Assert.DoesNotContain( iomedaeSpells, spell => spell.Spell.Id == fireball.Id );
    }

    [Fact]
    public void ResolveLoadout_AllowsRepeatedPreparedSpellButRequiresUniqueCantrips()
    {
        SpellDefinition[] cantrips = Enumerable
            .Range( 1, 5 )
            .Select( index => Create(
                $"cantrip_{index}",
                SpellKind.Cantrip,
                SpellRarity.Common,
                [ SpellTradition.Divine ] ) )
            .ToArray();
        SpellDefinition heal = Create(
            "heal",
            SpellKind.Spell,
            SpellRarity.Common,
            [ SpellTradition.Divine ] );
        IReadOnlyCollection<SpellDefinition> catalog = [ .. cantrips, heal ];
        Deity deity = CreateDeity( "iomedae", [] );
        string[] cantripIds = cantrips.Select( spell => spell.Id ).ToArray();

        ClericSpellLoadout result = ClericSpellLoadoutResolver.Resolve(
            deity,
            cantripIds,
            [ heal.Id, heal.Id ],
            catalog );

        Assert.Equal( cantripIds, result.CantripIds );
        Assert.Equal( [ heal.Id, heal.Id ], result.PreparedSpellIds );
        Assert.Throws<CharacterManagementException>( () => ClericSpellLoadoutResolver.Resolve(
            deity,
            [ cantripIds[ 0 ], cantripIds[ 0 ], cantripIds[ 2 ], cantripIds[ 3 ], cantripIds[ 4 ] ],
            [ heal.Id, heal.Id ],
            catalog ) );
    }

    [Fact]
    public void ResolveLoadout_RejectsSpellGrantedByAnotherDeityAndFocusSpell()
    {
        SpellDefinition[] cantrips = Enumerable
            .Range( 1, 5 )
            .Select( index => Create(
                $"cantrip_{index}",
                SpellKind.Cantrip,
                SpellRarity.Common,
                [ SpellTradition.Divine ] ) )
            .ToArray();
        SpellDefinition sureStrike = Create(
            "sure_strike",
            SpellKind.Spell,
            SpellRarity.Common,
            [ SpellTradition.Arcane ] );
        SpellDefinition focusSpell = Create(
            "fire_ray",
            SpellKind.Focus,
            SpellRarity.Uncommon,
            [ SpellTradition.Divine ] );
        Deity pharasma = CreateDeity( "pharasma", [] );
        IReadOnlyCollection<SpellDefinition> catalog = [ .. cantrips, sureStrike, focusSpell ];
        string[] cantripIds = cantrips.Select( spell => spell.Id ).ToArray();

        Assert.Throws<CharacterManagementException>( () => ClericSpellLoadoutResolver.Resolve(
            pharasma,
            cantripIds,
            [ sureStrike.Id, sureStrike.Id ],
            catalog ) );
        Assert.Throws<CharacterManagementException>( () => ClericSpellLoadoutResolver.Resolve(
            pharasma,
            cantripIds,
            [ focusSpell.Id, focusSpell.Id ],
            catalog ) );
    }

    private static SpellDefinition Create(
        string id,
        SpellKind kind,
        SpellRarity rarity,
        IReadOnlyList<SpellTradition> traditions )
    {
        return new SpellDefinition(
            $"spell.{id}",
            id,
            1,
            kind,
            traditions,
            [],
            rarity,
            new SourceReference( "Test", 1 ) );
    }

    private static Deity CreateDeity( string id, IReadOnlyList<DeityGrantedSpell> grantedSpells )
    {
        return new Deity(
            $"deity.{id}",
            id,
            new SourceReference( "Test", 1 ),
            true,
            "skill.religion",
            [ new DeityFavoredWeapon( "weapon.mace", "Mace", FavoredWeaponCategory.Simple ) ],
            [ DivineFont.Heal ],
            [],
            null,
            [ "domain.healing" ],
            grantedSpells );
    }
}

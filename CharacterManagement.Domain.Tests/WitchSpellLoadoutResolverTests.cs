using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace CharacterManagement.Domain.Tests;

public sealed class WitchSpellLoadoutResolverTests
{
    [Fact]
    public void Resolve_CreatesFamiliarStorageAndAllowsDuplicatePreparedSpell()
    {
        WitchPatron patron = CreatePatron();
        IReadOnlyCollection<SpellDefinition> catalog = CreateCatalog();

        WitchSpellLoadout result = Resolve( patron, catalog );

        Assert.Equal( 10, result.FamiliarCantripIds.Count );
        Assert.Equal( 5, result.FamiliarRankOneSpellIds.Count );
        Assert.Equal( "spell.command", result.PatronGrantedSpellId );
        Assert.Equal( [ "spell.known_1", "spell.known_1" ], result.PreparedSpellIds );
    }

    [Fact]
    public void Resolve_RejectsPreparedSpellUnknownToFamiliar()
    {
        WitchPatron patron = CreatePatron();
        IReadOnlyCollection<SpellDefinition> catalog = CreateCatalog();

        Assert.Throws<ArgumentException>( () => WitchSpellLoadoutResolver.Resolve(
            patron,
            null,
            CantripIds(),
            SpellIds(),
            CantripIds().Take( 5 ).ToArray(),
            [ "spell.command", "spell.unknown" ],
            "spell.phase_familiar",
            catalog ) );
    }

    [Fact]
    public void HexPackage_ReturnsPatronCantripChosenFocusHexAndPool()
    {
        WitchHexPackage result = WitchHexPackageResolver.Resolve(
            CreatePatron(),
            "spell.phase_familiar",
            CreateCatalog() );

        Assert.Equal( "spell.stoke_the_heart", result.PatronHexCantrip.Id );
        Assert.Equal( "spell.phase_familiar", result.FocusHex.Id );
        Assert.Equal( 1, result.MaximumFocusPoints );
    }

    private static WitchSpellLoadout Resolve(
        WitchPatron patron,
        IReadOnlyCollection<SpellDefinition> catalog )
    {
        return WitchSpellLoadoutResolver.Resolve(
            patron,
            null,
            CantripIds(),
            SpellIds(),
            CantripIds().Take( 5 ).ToArray(),
            [ "spell.known_1", "spell.known_1" ],
            "spell.phase_familiar",
            catalog );
    }

    private static WitchPatron CreatePatron()
    {
        return new WitchPatron(
            "witch_patron.test",
            "Test",
            SourceReference.Unknown,
            SpellTradition.Divine,
            new ClassSkillGrantDescriptor( "witch_patron.test.skill.patron", [ "skill.religion" ] ),
            [
                Benefit( "lesson.test", WitchPatronBenefitKind.Lesson ),
                Benefit( "spell.stoke_the_heart", WitchPatronBenefitKind.HexCantrip ),
                Benefit( "spell.command", WitchPatronBenefitKind.FamiliarSpell ),
                Benefit( "familiar_ability.test", WitchPatronBenefitKind.FamiliarAbility ),
            ] );
    }

    private static WitchPatronBenefitDescriptor Benefit( string id, WitchPatronBenefitKind kind )
    {
        return new WitchPatronBenefitDescriptor( id, kind, id, id, [] );
    }

    private static IReadOnlyCollection<SpellDefinition> CreateCatalog()
    {
        List<SpellDefinition> catalog = CantripIds()
            .Select( id => Spell( id, SpellKind.Cantrip, SpellRarity.Common ) )
            .Concat( SpellIds().Select( id => Spell( id, SpellKind.Spell, SpellRarity.Common ) ) )
            .ToList();
        catalog.Add( Spell( "spell.command", SpellKind.Spell, SpellRarity.Common ) );
        catalog.Add( Spell( "spell.stoke_the_heart", SpellKind.Cantrip, SpellRarity.Uncommon ) );
        catalog.Add( Spell( "spell.phase_familiar", SpellKind.Focus, SpellRarity.Uncommon ) );
        return catalog;
    }

    private static string[] CantripIds() =>
        Enumerable.Range( 1, 10 ).Select( index => $"spell.cantrip_{index}" ).ToArray();

    private static string[] SpellIds() =>
        Enumerable.Range( 1, 5 ).Select( index => $"spell.known_{index}" ).ToArray();

    private static SpellDefinition Spell( string id, SpellKind kind, SpellRarity rarity )
    {
        return new SpellDefinition(
            id,
            id,
            1,
            kind,
            [ SpellTradition.Divine ],
            [],
            rarity,
            SourceReference.Unknown );
    }
}

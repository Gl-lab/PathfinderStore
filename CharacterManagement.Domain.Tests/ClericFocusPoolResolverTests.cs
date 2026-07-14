using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace CharacterManagement.Domain.Tests;

public sealed class ClericFocusPoolResolverTests
{
    [Fact]
    public void Resolve_InitialDomainSpell_ReturnsOnePointPoolWithDomainInitiateSource()
    {
        ClericDomain domain = CreateDomain();
        SpellDefinition focusSpell = CreateSpell( SpellKind.Focus, 1 );

        ClericFocusPool result = ClericFocusPoolResolver.Resolve( domain, [ focusSpell ] );

        Assert.Equal( 1, result.MaximumFocusPoints );
        Assert.Same( focusSpell, result.FocusSpell );
        Assert.Equal(
            ClericFocusPoolResolver.DomainInitiateSourceGrantId,
            result.SourceGrantId );
    }

    [Theory]
    [InlineData( SpellKind.Spell, 1 )]
    [InlineData( SpellKind.Focus, 2 )]
    public void Resolve_NonInitialFocusDefinition_Throws( SpellKind kind, int rank )
    {
        Assert.Throws<CharacterManagementException>( () => ClericFocusPoolResolver.Resolve(
            CreateDomain(),
            [ CreateSpell( kind, rank ) ] ) );
    }

    [Fact]
    public void Resolve_MissingDefinition_Throws()
    {
        Assert.Throws<CharacterManagementException>( () => ClericFocusPoolResolver.Resolve(
            CreateDomain(),
            [] ) );
    }

    private static ClericDomain CreateDomain()
    {
        return new ClericDomain(
            "domain.fire",
            "Fire",
            SourceReference.Unknown,
            new SpellReference( "spell.fire_ray", "Fire Ray", 1, SpellKind.Focus ) );
    }

    private static SpellDefinition CreateSpell( SpellKind kind, int rank )
    {
        return new SpellDefinition(
            "spell.fire_ray",
            "Fire Ray",
            rank,
            kind,
            [ SpellTradition.Divine ],
            [ "Focus" ],
            SpellRarity.Uncommon,
            SourceReference.Unknown );
    }
}

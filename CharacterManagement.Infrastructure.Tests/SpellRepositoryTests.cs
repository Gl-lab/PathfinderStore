using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class SpellRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsCompleteUniquePlayerCoreLevelOneCatalog()
    {
        SpellRepository repository = new SpellRepository();

        IReadOnlyCollection<SpellDefinition> spells = repository.GetAll();

        Assert.Equal( 137, spells.Count );
        Assert.Equal( 137, spells.Select( spell => spell.Id ).Distinct( StringComparer.Ordinal ).Count() );
        Assert.Equal( 33, spells.Count( spell => spell.Kind == SpellKind.Cantrip ) );
        Assert.Equal( 58, spells.Count( spell => spell.Kind == SpellKind.Spell ) );
        Assert.Equal( 46, spells.Count( spell => spell.Kind == SpellKind.Focus ) );
        Assert.All( spells, spell => Assert.Equal( 1, spell.Rank ) );
    }

    [Fact]
    public void GetAll_ResolvesEveryRankOneDeityGrantAndInitialDomainFocusSpell()
    {
        SpellRepository spellRepository = new SpellRepository();
        DeityRepository deityRepository = new DeityRepository();
        ClericDomainRepository domainRepository = new ClericDomainRepository();
        IReadOnlyCollection<SpellDefinition> spells = spellRepository.GetAll();
        IReadOnlySet<string> spellIds = spells
            .Select( spell => spell.Id )
            .ToHashSet( StringComparer.Ordinal );

        string[] grantedSpellIds = deityRepository
            .GetAll()
            .SelectMany( deity => deity.GrantedSpells )
            .Where( spell => spell.Rank == 1 )
            .Select( spell => spell.Id )
            .Distinct( StringComparer.Ordinal )
            .ToArray();
        string[] focusSpellIds = domainRepository
            .GetAll()
            .Select( domain => domain.InitialFocusSpell.Id )
            .ToArray();

        Assert.All( grantedSpellIds, spellId => Assert.Contains( spellId, spellIds ) );
        Assert.All( focusSpellIds, spellId => Assert.Contains( spellId, spellIds ) );
        Assert.All(
            focusSpellIds,
            spellId => Assert.Equal( SpellKind.Focus, spellRepository.GetSpell( spellId ).Kind ) );
    }

    [Fact]
    public void GetAll_ContainsPlayerCoreMetadataAndExcludesPlayerCoreTwoSpells()
    {
        SpellRepository repository = new SpellRepository();

        SpellDefinition guidance = repository.GetSpell( "spell.guidance" );

        Assert.Equal( SpellRarity.Common, guidance.Rarity );
        Assert.Contains( SpellTradition.Divine, guidance.Traditions );
        Assert.Contains( "Cantrip", guidance.Traits );
        Assert.Equal( "Player Core", guidance.Source.Book );
        Assert.DoesNotContain( repository.GetAll(), spell => spell.Id == "spell.bullhorn" );
        Assert.DoesNotContain( repository.GetAll(), spell => spell.Id == "spell.haunting_hymn" );
        Assert.DoesNotContain( repository.GetAll(), spell => spell.Id == "spell.concordant_choir" );
    }

    [Theory]
    [InlineData( SpellTradition.Arcane, SpellKind.Cantrip, 19 )]
    [InlineData( SpellTradition.Arcane, SpellKind.Spell, 42 )]
    [InlineData( SpellTradition.Divine, SpellKind.Cantrip, 16 )]
    [InlineData( SpellTradition.Divine, SpellKind.Spell, 23 )]
    [InlineData( SpellTradition.Occult, SpellKind.Cantrip, 16 )]
    [InlineData( SpellTradition.Occult, SpellKind.Spell, 33 )]
    [InlineData( SpellTradition.Primal, SpellKind.Cantrip, 15 )]
    [InlineData( SpellTradition.Primal, SpellKind.Spell, 31 )]
    public void GetAll_ContainsExpectedCommonTraditionOptions(
        SpellTradition tradition,
        SpellKind kind,
        int expectedCount )
    {
        SpellRepository repository = new SpellRepository();

        IReadOnlyList<SpellDefinition> spells = SpellCatalogResolver.ResolveCommonOptions(
            repository.GetAll(),
            tradition,
            1,
            kind );

        Assert.Equal( expectedCount, spells.Count );
    }

    [Fact]
    public void Catalog_ProvidesExpectedFirstLevelOptionsForIomedaeCleric()
    {
        SpellRepository spellRepository = new SpellRepository();
        Deity iomedae = new DeityRepository().GetDeity( "deity.iomedae" );

        IReadOnlyList<ClericAvailableSpell> cantrips =
            ClericSpellAvailabilityResolver.ResolveCantrips( spellRepository.GetAll() );
        IReadOnlyList<ClericAvailableSpell> spells =
            ClericSpellAvailabilityResolver.ResolveRankOneSpells( iomedae, spellRepository.GetAll() );

        Assert.Equal( 16, cantrips.Count );
        Assert.Equal( 24, spells.Count );
        ClericAvailableSpell sureStrike = Assert.Single(
            spells,
            spell => spell.Spell.Id == "spell.sure_strike" );
        Assert.Equal( [ ClericSpellAccessSource.DeityGranted ], sureStrike.AccessSources );
        Assert.Equal( SpellTradition.Divine, sureStrike.EffectiveTradition );
    }
}

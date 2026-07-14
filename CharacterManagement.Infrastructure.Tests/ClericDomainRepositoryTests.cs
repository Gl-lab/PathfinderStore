using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class ClericDomainRepositoryTests
{
    [Fact]
    public void GetAll_ReturnsCompleteUniquePrimaryDomainCatalog()
    {
        ClericDomainRepository repository = new ClericDomainRepository();

        IReadOnlyCollection<ClericDomain> domains = repository.GetAll();

        Assert.Equal( 39, domains.Count );
        Assert.Equal( 39, domains.Select( domain => domain.Id ).Distinct().Count() );
        Assert.All( domains, domain =>
        {
            Assert.Equal( SpellKind.Focus, domain.InitialFocusSpell.Kind );
            Assert.Equal( 1, domain.InitialFocusSpell.Rank );
        } );
    }

    [Fact]
    public void GetAll_ResolvesEveryPrimaryDomainFromEligibleDeities()
    {
        ClericDomainRepository domainRepository = new ClericDomainRepository();
        DeityRepository deityRepository = new DeityRepository();

        string[] primaryDomainIds = deityRepository
            .GetAll()
            .Where( deity => deity.CanGrantClericPowers )
            .SelectMany( deity => deity.PrimaryDomainIds )
            .Distinct( StringComparer.Ordinal )
            .ToArray();

        Assert.Equal( 39, primaryDomainIds.Length );
        Assert.All(
            primaryDomainIds,
            domainId => Assert.NotNull( domainRepository.GetClericDomain( domainId ) ) );
    }

    [Theory]
    [InlineData( "domain.metal", "spell.serrate" )]
    [InlineData( "domain.wood", "spell.arms_of_nature" )]
    public void GetClericDomain_ElementalGreenFaithDomain_ReturnsInitialSpell(
        string domainId,
        string spellId )
    {
        ClericDomain domain = new ClericDomainRepository().GetClericDomain( domainId );

        Assert.Equal( spellId, domain.InitialFocusSpell.Id );
    }
}

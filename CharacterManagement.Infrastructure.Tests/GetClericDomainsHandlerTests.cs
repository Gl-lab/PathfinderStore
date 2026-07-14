using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.ClericDomains;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetClericDomainsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOrderedCatalogWithTypedInitialSpells()
    {
        GetClericDomainsHandler handler = new GetClericDomainsHandler(
            new ClericDomainRepository(),
            new SpellRepository() );

        IReadOnlyCollection<ClericDomainDto> result = await handler.Handle(
            new GetClericDomainsCommand(),
            CancellationToken.None );

        Assert.Equal( 39, result.Count );
        Assert.Equal(
            result.Select( domain => domain.Name ).OrderBy( name => name, StringComparer.Ordinal ),
            result.Select( domain => domain.Name ) );
        ClericDomainDto truth = Assert.Single( result, domain => domain.Id == "domain.truth" );
        Assert.Equal( "spell.word_of_truth", truth.InitialFocusSpell.Id );
        Assert.Equal( SpellKind.Focus, truth.InitialFocusSpell.Kind );
        Assert.Equal( 1, truth.InitialFocusPool.MaximumFocusPoints );
        Assert.Equal( "spell.word_of_truth", truth.InitialFocusPool.FocusSpell.Id );
        Assert.Equal(
            ClericFocusPoolResolver.DomainInitiateSourceGrantId,
            truth.InitialFocusPool.SourceGrantId );
        Assert.Contains(
            new ClericDoctrineRepository()
                .GetClericDoctrine( "cleric_doctrine.cloistered" )
                .Effects,
            effect => effect.Id == truth.InitialFocusPool.SourceGrantId );
    }
}

using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.ClericDomains;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetClericDomainsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOrderedCatalogWithTypedInitialSpells()
    {
        GetClericDomainsHandler handler = new GetClericDomainsHandler(
            new ClericDomainRepository() );

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
    }
}

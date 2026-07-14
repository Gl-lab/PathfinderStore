using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.UseCases.ArcaneSchools;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class GetArcaneSchoolsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompleteOrderedCatalogWithUnifiedException()
    {
        GetArcaneSchoolsHandler handler = new GetArcaneSchoolsHandler(
            new ArcaneSchoolRepository() );

        IReadOnlyCollection<ArcaneSchoolDto> result = await handler.Handle(
            new GetArcaneSchoolsCommand(),
            CancellationToken.None );

        Assert.Equal( 7, result.Count );
        Assert.Equal(
            result.Select( school => school.Name ).OrderBy( name => name ),
            result.Select( school => school.Name ) );
        ArcaneSchoolDto mentalism = Assert.Single(
            result.Where( school => school.Id == "arcane_school.mentalism" ) );
        Assert.True( mentalism.HasCurriculum );
        Assert.Contains( mentalism.CurriculumSpells, spell => spell.Rank == 9 );
        ArcaneSchoolDto unified = Assert.Single(
            result.Where( school => school.Id == "arcane_school.unified_magical_theory" ) );
        Assert.False( unified.HasCurriculum );
        Assert.Contains(
            unified.Benefits,
            benefit => benefit.Kind == ArcaneSchoolBenefitKind.ExtraClassFeat );
    }
}

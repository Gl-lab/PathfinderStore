using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IArcaneSchoolRepository
{
    IReadOnlyCollection<ArcaneSchool> GetAll();
    ArcaneSchool GetArcaneSchool( string arcaneSchoolId );
}

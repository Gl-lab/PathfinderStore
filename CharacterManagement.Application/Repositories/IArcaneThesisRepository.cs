using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IArcaneThesisRepository
{
    IReadOnlyCollection<ArcaneThesis> GetAll();
    ArcaneThesis GetArcaneThesis( string arcaneThesisId );
}

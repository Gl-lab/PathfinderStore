using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IDeityRepository
{
    IReadOnlyCollection<Deity> GetAll();
    Deity GetDeity( string deityId );
}

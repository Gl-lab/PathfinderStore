using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IClericDoctrineRepository
{
    IReadOnlyCollection<ClericDoctrine> GetAll();
    ClericDoctrine GetClericDoctrine( string clericDoctrineId );
}

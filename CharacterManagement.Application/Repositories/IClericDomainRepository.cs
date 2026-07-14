using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IClericDomainRepository
{
    IReadOnlyCollection<ClericDomain> GetAll();
    ClericDomain GetClericDomain( string clericDomainId );
}

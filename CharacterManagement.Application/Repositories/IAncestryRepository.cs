using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IAncestryRepository
{
    IReadOnlyCollection<Ancestry> GetAll();
    Ancestry GetAncestry( AncestryType ancestryType );
}

using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IAncestryRepository
{
    Ancestry GetAncestry( AncestryType ancestryType );
}
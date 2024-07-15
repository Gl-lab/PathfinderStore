using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Core.Repositories;

public interface IAncestryRepository
{
    Ancestry GetAncestry( AncestryType ancestryType );
}
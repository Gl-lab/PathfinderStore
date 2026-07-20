using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IFeatRepository
{
    IReadOnlyCollection<FeatDefinition> GetAll();
    FeatDefinition GetFeat( string featId );
}

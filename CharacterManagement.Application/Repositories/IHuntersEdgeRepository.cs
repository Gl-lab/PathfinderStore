using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IHuntersEdgeRepository
{
    IReadOnlyCollection<HuntersEdge> GetAll();
    HuntersEdge GetHuntersEdge( string huntersEdgeId );
}

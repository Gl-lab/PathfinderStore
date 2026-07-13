using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IRogueRacketRepository
{
    IReadOnlyCollection<RogueRacket> GetAll();
    RogueRacket GetRogueRacket( string rogueRacketId );
}

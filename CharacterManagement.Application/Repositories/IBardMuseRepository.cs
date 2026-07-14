using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IBardMuseRepository
{
    IReadOnlyCollection<BardMuse> GetAll();
    BardMuse GetBardMuse( string bardMuseId );
}

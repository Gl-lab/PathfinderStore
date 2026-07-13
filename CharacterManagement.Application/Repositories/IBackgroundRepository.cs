using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Repositories;

public interface IBackgroundRepository
{
    IReadOnlyCollection<Background> GetAll();
    Background GetBackground( string backgroundId );
}

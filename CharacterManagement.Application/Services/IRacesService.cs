using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Services;

public interface IRacesService
{
    Task<IReadOnlyCollection<Race>> RacesListAsync();
}
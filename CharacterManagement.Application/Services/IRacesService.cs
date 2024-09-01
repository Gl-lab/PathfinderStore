using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Services;

public interface IRacesService
{
    public Task<IReadOnlyCollection<Race>> RacesListAsync();
}
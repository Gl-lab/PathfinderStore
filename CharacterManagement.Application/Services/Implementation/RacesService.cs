using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Services.Implementation;

public sealed class RacesService : IRacesService
{
    private readonly IRacesRepository racesRepository;


    public RacesService( IRacesRepository racesRepository )
    {
        this.racesRepository = racesRepository;
    }

    public async Task<IReadOnlyCollection<Race>> RacesListAsync() => await racesRepository.ListAsync();
}
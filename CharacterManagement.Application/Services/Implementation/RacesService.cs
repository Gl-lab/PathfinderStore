using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Repositories;

namespace CharacterManagement.Application.Services.Implementation
{
    public sealed class RacesService : IRacesService
    {
        private readonly IRacesRepository racesRepository;


        public RacesService( IRacesRepository racesRepository )
        {
            this.racesRepository = racesRepository;
        }

        public async Task<IReadOnlyCollection<Race>> RacesListAsync()
        {
            return await racesRepository.ListAsync();
        }
    }
}
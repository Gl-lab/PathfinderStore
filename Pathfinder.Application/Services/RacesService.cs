using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Repositories;

namespace Pathfinder.Application.Services
{
    public sealed class RacesService : IRacesService
    {
        private readonly IRacesRepository racesRepository;
        private readonly IMapper mapper;

        public RacesService(IRacesRepository racesRepository,
            IMapper mapper)
        {
            this.racesRepository = racesRepository;
            this.mapper = mapper;
        }

        public async Task<IReadOnlyCollection<Race>> RacesListAsync()
        {
            return await racesRepository.ListAsync();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.DTO;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Application.Interfaces
{
    public interface IRacesService
    {
        public Task<IEnumerable<RaceDto>> RacesListAsync();
    }
}
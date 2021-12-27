using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Application.Interfaces
{
    public interface IRacesService
    {
        public Task<IReadOnlyCollection<Race>> RacesListAsync();
    }
}
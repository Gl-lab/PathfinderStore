using Pathfinder.Core.Entities.Account;

namespace CharacterManagement.Application.Services
{
    public interface IRacesService
    {
        public Task<IReadOnlyCollection<Race>> RacesListAsync();
    }
}
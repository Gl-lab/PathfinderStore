using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Repositories;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Infrastructure.Repository.Base;

namespace Pathfinder.Infrastructure.Repository
{
    public class RacesRepository: Repository<Race>, IRacesRepository
    {
        public RacesRepository(PgDbContext context) : base(context)
        {
        }
    }
}
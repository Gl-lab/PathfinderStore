using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Shared.Infrasturture.Repositories;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public class RacesRepository: Repository<Race>, IRacesRepository
{
    public RacesRepository(CharacterManagementDbContext context) : base(context)
    {
    }
}
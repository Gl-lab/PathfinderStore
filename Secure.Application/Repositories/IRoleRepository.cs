using Pathfinder.Secure.Domain.Authentication.Role;

namespace Pathfinder.Secure.Application.Repositories;

public interface IRoleRepository
{
    Task<ICollection<Role>> GetListAsync();
}
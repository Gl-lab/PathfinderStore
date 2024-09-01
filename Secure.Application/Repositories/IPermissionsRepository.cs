using Pathfinder.Secure.Domain.Authentication.Permissions;

namespace Pathfinder.Secure.Application.Repositories;

public interface IPermissionsRepository
{
    Task<ICollection<Permission>> GetListAsync();
}
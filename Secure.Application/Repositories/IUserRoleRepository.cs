using Pathfinder.Secure.Domain.Authentication.User;

namespace Pathfinder.Secure.Application.Repositories;

public interface IUserRoleRepository
{
    Task AddRangeAsync(IEnumerable<UserRole> userRoles);
}
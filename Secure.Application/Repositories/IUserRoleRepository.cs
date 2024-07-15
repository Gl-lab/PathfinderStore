using Authorization.Authentication.User;

namespace Authorization.Repositories
{
    public interface IUserRoleRepository
    {
        Task AddRangeAsync(IEnumerable<UserRole> userRoles);
    }
}

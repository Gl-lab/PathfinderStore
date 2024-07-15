using Authorization.Authentication.Permissions;

namespace Authorization.Repositories
{
    public interface IPermissionsRepository
    {
        Task<ICollection<Permission>> GetListAsync();
    }
}

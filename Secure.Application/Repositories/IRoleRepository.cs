using Authorization.Authentication.Role;

namespace Authorization.Repositories
{
    public interface IRoleRepository
    {
         Task<ICollection<Role>> GetListAsync();
    }
}

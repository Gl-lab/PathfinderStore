using Authorization.Authentication.Role;
using Authorization.Repositories;
using Microsoft.EntityFrameworkCore;
using Secure.Infrastructure.Data;

namespace Secure.Infrastructure.Repositories
{
    public class RoleRepository: IRoleRepository
    {
        private readonly SecureDbContext _dbContext;
        public RoleRepository(SecureDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ICollection<Role>> GetListAsync()
        {
            return await _dbContext
                .Roles
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}

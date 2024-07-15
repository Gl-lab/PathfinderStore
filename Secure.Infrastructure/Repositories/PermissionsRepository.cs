using Authorization.Authentication.Permissions;
using Authorization.Repositories;
using Microsoft.EntityFrameworkCore;
using Secure.Infrastructure.Data;

namespace Secure.Infrastructure.Repositories
{
    public class PermissionsRepository : IPermissionsRepository
    {
        private readonly SecureDbContext _dbContext;

        public PermissionsRepository(SecureDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ICollection<Permission>> GetListAsync()
        {
            return await _dbContext
                .Permission
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
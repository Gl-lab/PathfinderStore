using Authorization.Authentication.User;
using Authorization.Repositories;
using Secure.Infrastructure.Data;

namespace Secure.Infrastructure.Repositories
{
    public class UserRoleRepository: IUserRoleRepository
    {
        private readonly SecureDbContext _dbContext;
        public UserRoleRepository(SecureDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddRangeAsync(IEnumerable<UserRole> userRoles)
        {
            _dbContext.AddRange(userRoles);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

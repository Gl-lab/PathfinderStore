using Pathfinder.Secure.Application.Repositories;
using Pathfinder.Secure.Domain.Authentication.User;
using Pathfinder.Secure.Infrastructure.Data;

namespace Pathfinder.Secure.Infrastructure.Repositories;

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
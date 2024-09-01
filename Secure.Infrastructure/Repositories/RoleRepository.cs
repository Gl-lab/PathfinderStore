using Microsoft.EntityFrameworkCore;
using Pathfinder.Secure.Application.Repositories;
using Pathfinder.Secure.Domain.Authentication.Role;
using Pathfinder.Secure.Infrastructure.Data;

namespace Pathfinder.Secure.Infrastructure.Repositories;

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
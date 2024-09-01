using Microsoft.EntityFrameworkCore;
using Pathfinder.Secure.Application.Repositories;
using Pathfinder.Secure.Domain.Authentication.Permissions;
using Pathfinder.Secure.Infrastructure.Data;

namespace Pathfinder.Secure.Infrastructure.Repositories;

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
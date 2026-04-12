using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Shared.Infrasturture.Repositories;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository( CharacterManagementDbContext dbContext )
        : base( dbContext )
    {
    }

    public async Task<Account?> GetByUserIdAsync( int userId )
    {
        return await Table
           .Where( e => userId == e.UserId )
           .FirstOrDefaultAsync();
    }
}
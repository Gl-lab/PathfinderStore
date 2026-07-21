using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.CampaignManagement.Application.Campaigns;
using Pathfinder.Secure.Infrastructure.Data;

namespace Pathfinder.Web.Integration;

public sealed class CampaignUserDirectory : ICampaignUserDirectory
{
    private readonly SecureDbContext _dbContext;

    public CampaignUserDirectory( SecureDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<int?> FindUserIdByNameAsync(
        string userName,
        CancellationToken cancellationToken )
    {
        string normalizedUserName = userName.ToUpperInvariant();
        return await _dbContext.Users
            .AsNoTracking()
            .Where( user => user.NormalizedUserName == normalizedUserName )
            .Select( user => ( int? )user.Id )
            .SingleOrDefaultAsync( cancellationToken );
    }
}

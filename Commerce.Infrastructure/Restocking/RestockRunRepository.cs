using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Restocking;
using Pathfinder.Commerce.Domain.Restocking;
using Pathfinder.Commerce.Infrastructure.Data;

namespace Pathfinder.Commerce.Infrastructure.Restocking;

public sealed class RestockRunRepository : IRestockRunRepository
{
    private readonly CommerceDbContext _dbContext;

    public RestockRunRepository( CommerceDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public Task<RestockRun?> GetByIdentityAsync(
        int shopId,
        int restockPolicyId,
        int policyVersion,
        long seed,
        CancellationToken cancellationToken ) => _dbContext.RestockRuns
        .Include( run => run.Lines )
        .SingleOrDefaultAsync(
            run =>
                run.ShopId == shopId &&
                run.RestockPolicyId == restockPolicyId &&
                run.PolicyVersion == policyVersion &&
                run.Seed == seed,
            cancellationToken );

    public void Add( RestockRun run )
    {
        _dbContext.RestockRuns.Add( run );
    }

    public Task SaveChangesAsync( CancellationToken cancellationToken ) =>
        _dbContext.SaveChangesAsync( cancellationToken );
}

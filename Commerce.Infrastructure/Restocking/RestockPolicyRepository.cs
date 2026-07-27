using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Restocking;
using Pathfinder.Commerce.Domain.Restocking;
using Pathfinder.Commerce.Infrastructure.Data;

namespace Pathfinder.Commerce.Infrastructure.Restocking;

public sealed class RestockPolicyRepository : IRestockPolicyRepository
{
    private readonly CommerceDbContext _dbContext;

    public RestockPolicyRepository( CommerceDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public Task<RestockPolicy?> GetByShopAsync(
        int shopId,
        CancellationToken cancellationToken ) => _dbContext.RestockPolicies
        .Include( policy => policy.Revisions )
        .SingleOrDefaultAsync( policy => policy.ShopId == shopId, cancellationToken );

    public void Add( RestockPolicy policy )
    {
        _dbContext.RestockPolicies.Add( policy );
    }

    public Task SaveChangesAsync( CancellationToken cancellationToken ) =>
        _dbContext.SaveChangesAsync( cancellationToken );
}

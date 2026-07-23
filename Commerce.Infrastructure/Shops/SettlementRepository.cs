using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Shops;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Commerce.Infrastructure.Data;

namespace Pathfinder.Commerce.Infrastructure.Shops;

public sealed class SettlementRepository : ISettlementRepository
{
    private readonly CommerceDbContext _dbContext;

    public SettlementRepository( CommerceDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public Task<Settlement?> GetAsync(
        int settlementId,
        CancellationToken cancellationToken ) => _dbContext.Settlements
        .Include( settlement => settlement.Shops )
        .SingleOrDefaultAsync( settlement => settlement.Id == settlementId, cancellationToken );

    public void Add( Settlement settlement )
    {
        _dbContext.Settlements.Add( settlement );
    }

    public Task SaveChangesAsync( CancellationToken cancellationToken ) =>
        _dbContext.SaveChangesAsync( cancellationToken );
}

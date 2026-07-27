using Microsoft.EntityFrameworkCore;
using Pathfinder.Commerce.Application.Administration;
using Pathfinder.Commerce.Domain.Administration;
using Pathfinder.Commerce.Infrastructure.Data;

namespace Pathfinder.Commerce.Infrastructure.Administration;

public sealed class CommerceAdminOperationRepository : ICommerceAdminOperationRepository
{
    private readonly CommerceDbContext _dbContext;

    public CommerceAdminOperationRepository( CommerceDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public Task<CommerceAdminOperation?> GetAsync(
        int campaignId,
        Guid operationId,
        CancellationToken cancellationToken ) => _dbContext.CommerceAdminOperations
        .SingleOrDefaultAsync(
            operation =>
                operation.CampaignId == campaignId &&
                operation.OperationId == operationId,
            cancellationToken );

    public void Add( CommerceAdminOperation operation )
    {
        _dbContext.CommerceAdminOperations.Add( operation );
    }
}

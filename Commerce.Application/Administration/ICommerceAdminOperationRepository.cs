using Pathfinder.Commerce.Domain.Administration;

namespace Pathfinder.Commerce.Application.Administration;

public interface ICommerceAdminOperationRepository
{
    Task<CommerceAdminOperation?> GetAsync(
        int campaignId,
        Guid operationId,
        CancellationToken cancellationToken );

    void Add( CommerceAdminOperation operation );
}

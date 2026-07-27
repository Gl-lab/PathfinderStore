using Pathfinder.Commerce.Domain.Restocking;

namespace Pathfinder.Commerce.Application.Restocking;

public interface IRestockRunRepository
{
    Task<RestockRun?> GetByKeyAsync(
        Guid runKey,
        CancellationToken cancellationToken );
    Task<RestockRun?> GetByIdentityAsync(
        int shopId,
        int restockPolicyId,
        int policyVersion,
        long seed,
        CancellationToken cancellationToken );
    void Add( RestockRun run );
    Task SaveChangesAsync( CancellationToken cancellationToken );
}

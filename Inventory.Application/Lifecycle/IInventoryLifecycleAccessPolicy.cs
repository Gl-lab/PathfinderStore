using Pathfinder.Inventory.Domain.Containers;

namespace Pathfinder.Inventory.Application.Lifecycle;

public interface IInventoryLifecycleAccessPolicy
{
    Task<bool> CanMutateAsync(
        int campaignId,
        int actingUserId,
        InventoryContainerOwnerKind ownerKind,
        int ownerId,
        CancellationToken cancellationToken );
}

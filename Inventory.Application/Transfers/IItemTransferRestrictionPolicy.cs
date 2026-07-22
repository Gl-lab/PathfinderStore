namespace Pathfinder.Inventory.Application.Transfers;

public interface IItemTransferRestrictionPolicy
{
    Task<bool> IsEquippedAsync(
        int characterId,
        Guid itemInstanceKey,
        CancellationToken cancellationToken );
}

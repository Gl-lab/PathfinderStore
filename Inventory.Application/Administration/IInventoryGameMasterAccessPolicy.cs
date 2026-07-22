namespace Pathfinder.Inventory.Application.Administration;

public interface IInventoryGameMasterAccessPolicy
{
    Task<bool> IsGameMasterAsync(
        int campaignId,
        int actingUserId,
        CancellationToken cancellationToken );
}

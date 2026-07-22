namespace Pathfinder.Inventory.Application.Transfers;

public interface IPartyTransferAccessPolicy
{
    Task<PartyTransferAccess> GetAccessAsync(
        int campaignId,
        int actingUserId,
        int sourceCharacterId,
        int destinationCharacterId,
        CancellationToken cancellationToken );
}

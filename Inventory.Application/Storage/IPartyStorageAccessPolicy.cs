namespace Pathfinder.Inventory.Application.Storage;

public interface IPartyStorageAccessPolicy
{
    Task<PartyStorageAccess> GetAccessAsync(
        int campaignId,
        int actingUserId,
        int characterId,
        CancellationToken cancellationToken );
}

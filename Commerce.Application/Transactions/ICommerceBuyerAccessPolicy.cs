namespace Pathfinder.Commerce.Application.Transactions;

public interface ICommerceBuyerAccessPolicy
{
    Task<bool> ControlsCharacterAsync(
        int campaignId,
        int actingUserId,
        int characterId,
        CancellationToken cancellationToken );
}

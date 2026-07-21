namespace Pathfinder.CharacterManagement.Application.Access;

public interface ICharacterCampaignAccessPolicy
{
    Task<CharacterCampaignAccess> GetAccessAsync(
        int campaignId,
        int userId,
        int characterId,
        CancellationToken cancellationToken );
}

public sealed record CharacterCampaignAccess( bool CanView, bool CanAct )
{
    public static CharacterCampaignAccess Denied { get; } = new CharacterCampaignAccess( false, false );
}

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public interface ICampaignCharacterDirectory
{
    Task<CampaignCharacterReference?> GetOwnedCharacterAsync(
        int characterId,
        int userId,
        CancellationToken cancellationToken );

    Task<IReadOnlyCollection<CampaignCharacterReference>> GetOwnedCharactersAsync(
        int userId,
        CancellationToken cancellationToken );
}

public sealed record CampaignCharacterReference( int Id, string Name );

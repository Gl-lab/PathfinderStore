using Pathfinder.CampaignManagement.Application.Campaigns;

namespace Pathfinder.CampaignManagement.Infrastructure.Tests.TestSupport;

internal sealed class FakeCampaignCharacterDirectory : ICampaignCharacterDirectory
{
    private readonly IReadOnlyCollection<( int UserId, CampaignCharacterReference Character )> _characters;

    public FakeCampaignCharacterDirectory(
        IReadOnlyCollection<( int UserId, CampaignCharacterReference Character )> characters )
    {
        _characters = characters;
    }

    public Task<CampaignCharacterReference?> GetOwnedCharacterAsync(
        int characterId,
        int userId,
        CancellationToken cancellationToken )
    {
        CampaignCharacterReference? character = _characters
            .Where( item =>
                item.UserId == userId &&
                item.Character.Id == characterId )
            .Select( item => item.Character )
            .SingleOrDefault();
        return Task.FromResult( character );
    }

    public Task<IReadOnlyCollection<CampaignCharacterReference>> GetOwnedCharactersAsync(
        int userId,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<CampaignCharacterReference> characters = _characters
            .Where( item => item.UserId == userId )
            .Select( item => item.Character )
            .ToArray();
        return Task.FromResult( characters );
    }
}

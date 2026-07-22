using Pathfinder.CharacterManagement.Application.Access;

namespace CharacterManagement.Infrastructure.Tests.TestSupport;

internal sealed class FakeCharacterCampaignAccessPolicy : ICharacterCampaignAccessPolicy
{
    private readonly int _allowedUserId;
    private readonly int _allowedCampaignId;
    private readonly bool _canAct;

    public FakeCharacterCampaignAccessPolicy(
        int allowedUserId,
        int allowedCampaignId,
        bool canAct = true )
    {
        _allowedUserId = allowedUserId;
        _allowedCampaignId = allowedCampaignId;
        _canAct = canAct;
    }

    public Task<CharacterCampaignAccess> GetAccessAsync(
        int campaignId,
        int userId,
        int characterId,
        CancellationToken cancellationToken )
    {
        bool allowed = userId == _allowedUserId && campaignId == _allowedCampaignId;
        return Task.FromResult( new CharacterCampaignAccess( allowed, allowed && _canAct ) );
    }
}

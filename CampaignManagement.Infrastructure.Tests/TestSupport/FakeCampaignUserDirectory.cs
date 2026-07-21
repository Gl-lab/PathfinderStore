using Pathfinder.CampaignManagement.Application.Campaigns;

namespace Pathfinder.CampaignManagement.Infrastructure.Tests.TestSupport;

internal sealed class FakeCampaignUserDirectory : ICampaignUserDirectory
{
    private readonly IReadOnlyDictionary<string, int> _users;

    public FakeCampaignUserDirectory( IReadOnlyDictionary<string, int> users )
    {
        _users = users;
    }

    public Task<int?> FindUserIdByNameAsync(
        string userName,
        CancellationToken cancellationToken )
    {
        int? userId = _users.TryGetValue( userName, out int value ) ? value : null;
        return Task.FromResult( userId );
    }
}

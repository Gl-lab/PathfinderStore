using MediatR;

namespace Pathfinder.CampaignManagement.Application.Campaigns;

public sealed class GetCampaignCharactersHandler
    : IRequestHandler<GetCampaignCharactersQuery, IReadOnlyCollection<CampaignCharacterReference>>
{
    private readonly ICampaignCharacterDirectory _characterDirectory;

    public GetCampaignCharactersHandler( ICampaignCharacterDirectory characterDirectory )
    {
        _characterDirectory = characterDirectory;
    }

    public async Task<IReadOnlyCollection<CampaignCharacterReference>> Handle(
        GetCampaignCharactersQuery request,
        CancellationToken cancellationToken )
    {
        return await _characterDirectory.GetOwnedCharactersAsync(
            request.UserId,
            cancellationToken );
    }
}

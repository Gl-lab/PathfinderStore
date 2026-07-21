using Pathfinder.CampaignManagement.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.CampaignManagement.Domain.Campaigns;

public sealed class CampaignPartyCharacter : Entity
{
    private CampaignPartyCharacter()
    {
    }

    public int CampaignPartyId { get; private set; }
    public int CharacterId { get; private set; }
    public int ControlledByUserId { get; private set; }
    public DateTimeOffset AssignedAtUtc { get; private set; }

    internal static CampaignPartyCharacter Create(
        int characterId,
        int controlledByUserId,
        DateTimeOffset assignedAtUtc )
    {
        if ( characterId <= 0 )
        {
            throw new CampaignManagementException( "CharacterId must be greater than zero." );
        }

        if ( controlledByUserId <= 0 )
        {
            throw new CampaignManagementException( "Controlled user id must be greater than zero." );
        }

        if ( assignedAtUtc.Offset != TimeSpan.Zero )
        {
            throw new CampaignManagementException( "Character assignment timestamp must use UTC." );
        }

        return new CampaignPartyCharacter
        {
            CharacterId = characterId,
            ControlledByUserId = controlledByUserId,
            AssignedAtUtc = assignedAtUtc,
        };
    }
}

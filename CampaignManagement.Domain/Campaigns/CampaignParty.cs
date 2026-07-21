using Pathfinder.CampaignManagement.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.CampaignManagement.Domain.Campaigns;

public sealed class CampaignParty : Entity
{
    public const int NameMaxLength = 200;

    private readonly List<CampaignPartyCharacter> _characters = [];

    private CampaignParty()
    {
    }

    public int CampaignId { get; private set; }
    public string Name { get; private set; } = String.Empty;
    public CampaignPartyStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? ArchivedAtUtc { get; private set; }
    public IReadOnlyList<CampaignPartyCharacter> Characters { get => _characters; }

    internal static CampaignParty Create( string name, DateTimeOffset createdAtUtc )
    {
        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new CampaignManagementException( "Party name cannot be empty." );
        }

        string normalizedName = name.Trim();
        if ( normalizedName.Length > NameMaxLength )
        {
            throw new CampaignManagementException( $"Party name cannot exceed {NameMaxLength} characters." );
        }

        EnsureUtc( createdAtUtc, "Party creation timestamp" );
        return new CampaignParty
        {
            Name = normalizedName,
            Status = CampaignPartyStatus.Active,
            CreatedAtUtc = createdAtUtc,
        };
    }

    internal CampaignPartyCharacter AssignCharacter(
        int characterId,
        int controlledByUserId,
        DateTimeOffset assignedAtUtc )
    {
        if ( Status != CampaignPartyStatus.Active )
        {
            throw new CampaignManagementException( "Characters cannot be assigned to an archived party." );
        }

        if ( _characters.Any( character => character.CharacterId == characterId ) )
        {
            throw new CampaignManagementException( "Character is already assigned to this party." );
        }

        CampaignPartyCharacter character = CampaignPartyCharacter.Create(
            characterId,
            controlledByUserId,
            assignedAtUtc );
        _characters.Add( character );
        return character;
    }

    private static void EnsureUtc( DateTimeOffset value, string fieldName )
    {
        if ( value.Offset != TimeSpan.Zero )
        {
            throw new CampaignManagementException( $"{fieldName} must use UTC." );
        }
    }
}

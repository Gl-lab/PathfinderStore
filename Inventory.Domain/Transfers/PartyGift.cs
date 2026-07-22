using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Inventory.Domain.Transfers;

public sealed class PartyGift : Entity, IAggregateRoot
{
    private PartyGift()
    {
    }

    public Guid GiftKey { get; private set; }
    public int CampaignId { get; private set; }
    public int PartyId { get; private set; }
    public int SourceCharacterId { get; private set; }
    public int DestinationCharacterId { get; private set; }
    public Guid ItemInstanceKey { get; private set; }
    public int ExpectedItemVersion { get; private set; }
    public PartyGiftStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? AcceptedAtUtc { get; private set; }
    public Guid? AcceptanceOperationId { get; private set; }

    public static PartyGift Create(
        Guid giftKey,
        int campaignId,
        int partyId,
        int sourceCharacterId,
        int destinationCharacterId,
        Guid itemInstanceKey,
        int expectedItemVersion,
        DateTimeOffset createdAtUtc,
        DateTimeOffset expiresAtUtc )
    {
        if ( ( giftKey == Guid.Empty ) || ( itemInstanceKey == Guid.Empty ) )
        {
            throw new InventoryException( "Gift and item keys cannot be empty." );
        }

        if ( ( campaignId <= 0 ) ||
             ( partyId <= 0 ) ||
             ( sourceCharacterId <= 0 ) ||
             ( destinationCharacterId <= 0 ) )
        {
            throw new InventoryException( "Gift campaign, party, and character ids must be positive." );
        }

        if ( sourceCharacterId == destinationCharacterId )
        {
            throw new InventoryException( "A character cannot gift an item to itself." );
        }

        if ( expectedItemVersion < 0 )
        {
            throw new InventoryException( "Expected item version cannot be negative." );
        }

        EnsureUtc( createdAtUtc, "Gift creation timestamp" );
        EnsureUtc( expiresAtUtc, "Gift expiration timestamp" );
        if ( expiresAtUtc <= createdAtUtc )
        {
            throw new InventoryException( "Gift expiration must be after creation." );
        }

        return new PartyGift
        {
            GiftKey = giftKey,
            CampaignId = campaignId,
            PartyId = partyId,
            SourceCharacterId = sourceCharacterId,
            DestinationCharacterId = destinationCharacterId,
            ItemInstanceKey = itemInstanceKey,
            ExpectedItemVersion = expectedItemVersion,
            Status = PartyGiftStatus.Pending,
            CreatedAtUtc = createdAtUtc,
            ExpiresAtUtc = expiresAtUtc,
        };
    }

    public void EnsureMatches(
        int campaignId,
        int sourceCharacterId,
        int destinationCharacterId,
        Guid itemInstanceKey,
        int expectedItemVersion )
    {
        if ( ( CampaignId != campaignId ) ||
             ( SourceCharacterId != sourceCharacterId ) ||
             ( DestinationCharacterId != destinationCharacterId ) ||
             ( ItemInstanceKey != itemInstanceKey ) ||
             ( ExpectedItemVersion != expectedItemVersion ) )
        {
            throw new InventoryException( "Gift key was already used for another proposal." );
        }
    }

    public bool Accept(
        int currentItemVersion,
        Guid operationId,
        DateTimeOffset acceptedAtUtc )
    {
        if ( operationId == Guid.Empty )
        {
            throw new InventoryException( "Gift acceptance operation id cannot be empty." );
        }

        if ( Status == PartyGiftStatus.Accepted )
        {
            if ( AcceptanceOperationId != operationId )
            {
                throw new InventoryException( "Gift was already accepted by another operation." );
            }

            return false;
        }

        EnsureUtc( acceptedAtUtc, "Gift acceptance timestamp" );
        if ( Status != PartyGiftStatus.Pending )
        {
            throw new InventoryException( "Gift is not pending." );
        }

        if ( acceptedAtUtc > ExpiresAtUtc )
        {
            throw new InventoryException( "Gift has expired." );
        }

        if ( currentItemVersion != ExpectedItemVersion )
        {
            throw new InventoryException( "Gift item changed after the proposal was created." );
        }

        Status = PartyGiftStatus.Accepted;
        AcceptedAtUtc = acceptedAtUtc;
        AcceptanceOperationId = operationId;
        return true;
    }

    private static void EnsureUtc( DateTimeOffset value, string fieldName )
    {
        if ( value.Offset != TimeSpan.Zero )
        {
            throw new InventoryException( $"{fieldName} must use UTC." );
        }
    }
}

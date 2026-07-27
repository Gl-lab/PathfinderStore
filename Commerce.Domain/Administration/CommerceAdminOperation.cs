using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Commerce.Domain.Offers;
using Pathfinder.Commerce.Domain.Shops;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Administration;

public sealed class CommerceAdminOperation : Entity, IAggregateRoot
{
    public const int ActionKindMaxLength = 64;
    public const int PayloadHashMaxLength = 64;

    private CommerceAdminOperation()
    {
    }

    public int CampaignId { get; private set; }
    public Guid OperationId { get; private set; }
    public string ActionKind { get; private set; } = String.Empty;
    public string PayloadHash { get; private set; } = String.Empty;
    public int? SettlementId { get; private set; }
    public int? ShopId { get; private set; }
    public int? OfferId { get; private set; }
    public Settlement? Settlement { get; private set; }
    public Shop? Shop { get; private set; }
    public ShopOffer? Offer { get; private set; }
    public int PerformedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static CommerceAdminOperation Create(
        int campaignId,
        Guid operationId,
        string actionKind,
        string payloadHash,
        int performedByUserId,
        DateTimeOffset createdAtUtc,
        Settlement? settlement = null,
        Shop? shop = null,
        ShopOffer? offer = null )
    {
        if ( campaignId <= 0 || operationId == Guid.Empty || performedByUserId <= 0 )
        {
            throw new CommerceException( "Admin operation identity is invalid." );
        }

        if ( createdAtUtc.Offset != TimeSpan.Zero )
        {
            throw new CommerceException( "Admin operation timestamp must use UTC." );
        }

        return new CommerceAdminOperation
        {
            CampaignId = campaignId,
            OperationId = operationId,
            ActionKind = NormalizeRequired(
                actionKind,
                ActionKindMaxLength,
                "Admin operation action kind" ),
            PayloadHash = NormalizeRequired(
                payloadHash,
                PayloadHashMaxLength,
                "Admin operation payload hash" ),
            SettlementId = settlement?.Id,
            ShopId = shop?.Id,
            OfferId = offer?.Id,
            Settlement = settlement,
            Shop = shop,
            Offer = offer,
            PerformedByUserId = performedByUserId,
            CreatedAtUtc = createdAtUtc,
        };
    }

    public void EnsureReplayMatches( string actionKind, string payloadHash )
    {
        if ( !String.Equals( ActionKind, actionKind, StringComparison.Ordinal ) ||
             !String.Equals( PayloadHash, payloadHash, StringComparison.Ordinal ) )
        {
            throw new CommerceException(
                "Operation id was already used for a different admin command." );
        }
    }

    private static string NormalizeRequired( string value, int maxLength, string fieldName )
    {
        string normalized = value?.Trim() ?? String.Empty;
        if ( normalized.Length == 0 || normalized.Length > maxLength )
        {
            throw new CommerceException( $"{fieldName} is invalid." );
        }

        return normalized;
    }
}

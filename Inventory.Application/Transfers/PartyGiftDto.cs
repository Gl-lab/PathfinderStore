using Pathfinder.Inventory.Domain.Transfers;

namespace Pathfinder.Inventory.Application.Transfers;

public sealed record PartyGiftDto(
    Guid GiftKey,
    int CampaignId,
    int PartyId,
    int SourceCharacterId,
    int DestinationCharacterId,
    Guid ItemInstanceKey,
    int ExpectedItemVersion,
    PartyGiftStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset? AcceptedAtUtc,
    Guid? AcceptanceOperationId );

internal static class PartyGiftMappings
{
    internal static PartyGiftDto ToDto( this PartyGift gift ) => new(
        gift.GiftKey,
        gift.CampaignId,
        gift.PartyId,
        gift.SourceCharacterId,
        gift.DestinationCharacterId,
        gift.ItemInstanceKey,
        gift.ExpectedItemVersion,
        gift.Status,
        gift.CreatedAtUtc,
        gift.ExpiresAtUtc,
        gift.AcceptedAtUtc,
        gift.AcceptanceOperationId );
}

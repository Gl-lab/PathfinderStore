namespace Pathfinder.Commerce.Application.Restocking;

public sealed record RestockPolicyDto(
    int Id,
    int CampaignId,
    int ShopId,
    string Name,
    int CurrentVersion,
    IReadOnlyCollection<RestockPolicyRevisionDto> Revisions );

public sealed record RestockPolicyRevisionDto(
    int Version,
    int TargetOfferCount,
    int CreatedByUserId,
    DateTimeOffset CreatedAtUtc );

public sealed record CreateRestockPolicyRequest(
    int CampaignId,
    int ShopId,
    string Name,
    int TargetOfferCount,
    int ActingUserId );

public sealed record ReviseRestockPolicyRequest(
    int CampaignId,
    int ShopId,
    int ExpectedVersion,
    int TargetOfferCount,
    int ActingUserId );

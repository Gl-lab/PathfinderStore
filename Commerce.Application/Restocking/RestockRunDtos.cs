using Pathfinder.Commerce.Domain.Restocking;

namespace Pathfinder.Commerce.Application.Restocking;

public sealed record RestockRunDto(
    Guid RunKey,
    int CampaignId,
    int ShopId,
    int RestockPolicyId,
    int PolicyVersion,
    long Seed,
    RestockRunStatus Status,
    long TotalPriceCopper,
    IReadOnlyCollection<RestockRunLineDto> Lines );

public sealed record RestockRunLineDto(
    int Sequence,
    int ItemConfigurationId,
    int Quantity,
    long UnitPriceCopper,
    RestockItemKind Kind );

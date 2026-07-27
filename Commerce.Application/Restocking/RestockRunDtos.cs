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
    int? CompletedByUserId,
    DateTimeOffset? CompletedAtUtc,
    long TotalPriceCopper,
    IReadOnlyCollection<RestockRunLineDto> Lines );

public sealed record RestockRunLineDto(
    int Sequence,
    int ItemConfigurationId,
    int Quantity,
    long UnitPriceCopper,
    RestockItemKind Kind,
    Guid? PublishedOfferKey,
    Guid? PublishedItemInstanceKey );

internal static class RestockRunMappings
{
    internal static RestockRunDto ToDto( this RestockRun run ) => new RestockRunDto(
        run.RunKey,
        run.CampaignId,
        run.ShopId,
        run.RestockPolicyId,
        run.PolicyVersion,
        run.Seed,
        run.Status,
        run.CompletedByUserId,
        run.CompletedAtUtc,
        run.TotalPriceCopper,
        run.Lines
            .OrderBy( line => line.Sequence )
            .Select( line => new RestockRunLineDto(
                line.Sequence,
                line.ItemConfigurationId,
                line.Quantity,
                line.UnitPriceCopper,
                line.Kind,
                line.PublishedOfferKey,
                line.PublishedItemInstanceKey ) )
            .ToArray() );
}

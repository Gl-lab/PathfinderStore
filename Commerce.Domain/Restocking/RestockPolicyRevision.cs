using Pathfinder.Commerce.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Commerce.Domain.Restocking;

public sealed class RestockPolicyRevision : Entity
{
    private RestockPolicyRevision()
    {
    }

    public int RestockPolicyId { get; private set; }
    public int Version { get; private set; }
    public int TargetOfferCount { get; private set; }
    public int MinimumItemLevel { get; private set; }
    public int MaximumItemLevel { get; private set; }
    public long BudgetCopper { get; private set; }
    public RestockItemRarity AllowedRarities { get; private set; }
    public RestockItemAccess AllowedAccess { get; private set; }
    public RestockItemCategory AllowedCategories { get; private set; }
    public int CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    internal static RestockPolicyRevision Create(
        int version,
        int targetOfferCount,
        RestockPolicyConstraints constraints,
        int createdByUserId,
        DateTimeOffset createdAtUtc )
    {
        if ( version <= 0 )
        {
            throw new CommerceException( "Restock policy revision version must be greater than zero." );
        }

        if ( targetOfferCount <= 0 )
        {
            throw new CommerceException( "Restock target offer count must be greater than zero." );
        }

        ArgumentNullException.ThrowIfNull( constraints );

        if ( createdByUserId <= 0 )
        {
            throw new CommerceException( "Restock policy revision author must be greater than zero." );
        }

        if ( createdAtUtc.Offset != TimeSpan.Zero )
        {
            throw new CommerceException( "Restock policy revision timestamp must use UTC." );
        }

        return new RestockPolicyRevision
        {
            Version = version,
            TargetOfferCount = targetOfferCount,
            MinimumItemLevel = constraints.MinimumItemLevel,
            MaximumItemLevel = constraints.MaximumItemLevel,
            BudgetCopper = constraints.BudgetCopper,
            AllowedRarities = constraints.AllowedRarities,
            AllowedAccess = constraints.AllowedAccess,
            AllowedCategories = constraints.AllowedCategories,
            CreatedByUserId = createdByUserId,
            CreatedAtUtc = createdAtUtc,
        };
    }

    public bool Allows( RestockCandidate candidate, long remainingBudgetCopper )
    {
        ArgumentNullException.ThrowIfNull( candidate );
        return candidate.ItemConfigurationId > 0 &&
               IsSingleFlag( ( long )candidate.Rarity ) &&
               IsSingleFlag( ( long )candidate.Access ) &&
               IsSingleFlag( ( long )candidate.Category ) &&
               candidate.Level >= MinimumItemLevel &&
               candidate.Level <= MaximumItemLevel &&
               candidate.UnitPriceCopper >= 0 &&
               candidate.UnitPriceCopper <= remainingBudgetCopper &&
               candidate.UnitPriceCopper <= BudgetCopper &&
               AllowedRarities.HasFlag( candidate.Rarity ) &&
               AllowedAccess.HasFlag( candidate.Access ) &&
               AllowedCategories.HasFlag( candidate.Category );
    }

    private static bool IsSingleFlag( long value ) =>
        ( value > 0 ) && (( value & ( value - 1 ) ) == 0);
}

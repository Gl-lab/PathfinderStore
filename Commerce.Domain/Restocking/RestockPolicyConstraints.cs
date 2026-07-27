using Pathfinder.Commerce.Domain.Exceptions;

namespace Pathfinder.Commerce.Domain.Restocking;

public sealed record RestockPolicyConstraints
{
    public RestockPolicyConstraints(
        int minimumItemLevel,
        int maximumItemLevel,
        long budgetCopper,
        RestockItemRarity allowedRarities,
        RestockItemAccess allowedAccess,
        RestockItemCategory allowedCategories )
    {
        if ( minimumItemLevel < 0 ||
             maximumItemLevel < minimumItemLevel ||
             maximumItemLevel > 30 )
        {
            throw new CommerceException(
                "Restock item level range must be ordered and between 0 and 30." );
        }

        if ( budgetCopper < 0 )
        {
            throw new CommerceException( "Restock budget cannot be negative." );
        }

        EnsureFlags( allowedRarities, RestockItemRarity.All, "rarity" );
        EnsureFlags( allowedAccess, RestockItemAccess.All, "access" );
        EnsureFlags( allowedCategories, RestockItemCategory.All, "category" );
        MinimumItemLevel = minimumItemLevel;
        MaximumItemLevel = maximumItemLevel;
        BudgetCopper = budgetCopper;
        AllowedRarities = allowedRarities;
        AllowedAccess = allowedAccess;
        AllowedCategories = allowedCategories;
    }

    public int MinimumItemLevel { get; }
    public int MaximumItemLevel { get; }
    public long BudgetCopper { get; }
    public RestockItemRarity AllowedRarities { get; }
    public RestockItemAccess AllowedAccess { get; }
    public RestockItemCategory AllowedCategories { get; }

    private static void EnsureFlags<TEnum>( TEnum value, TEnum all, string fieldName )
        where TEnum : struct, Enum
    {
        long numericValue = Convert.ToInt64( value );
        long allValue = Convert.ToInt64( all );
        if ( numericValue <= 0 || (numericValue & ~allValue) != 0 )
        {
            throw new CommerceException( $"Restock allowed {fieldName} flags are invalid." );
        }
    }
}

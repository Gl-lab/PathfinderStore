namespace Pathfinder.CharacterManagement.Domain.Entity;

public interface IAncestryChoiceAvailabilityPolicy
{
    bool IsAvailable( Heritage heritage );
    bool IsAvailable( AncestryFeat ancestryFeat );
}

public sealed class CommonAncestryChoiceAvailabilityPolicy : IAncestryChoiceAvailabilityPolicy
{
    public bool IsAvailable( Heritage heritage )
    {
        ArgumentNullException.ThrowIfNull( heritage );

        return heritage.Rarity == AncestryChoiceRarity.Common;
    }

    public bool IsAvailable( AncestryFeat ancestryFeat )
    {
        ArgumentNullException.ThrowIfNull( ancestryFeat );

        return ancestryFeat.Rarity == AncestryChoiceRarity.Common;
    }
}

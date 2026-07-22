using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Rules;

public sealed class DurabilityComponent : Entity
{
    private DurabilityComponent()
    {
    }

    public int ItemRevisionId { get; private set; }
    public int Hardness { get; private set; }
    public int MaximumHitPoints { get; private set; }
    public int BrokenThreshold { get; private set; }

    public static DurabilityComponent Create(
        int hardness,
        int maximumHitPoints,
        int brokenThreshold )
    {
        if ( hardness < 0 )
        {
            throw new ItemCatalogException( "Item Hardness cannot be negative." );
        }

        if ( maximumHitPoints <= 0 )
        {
            throw new ItemCatalogException( "Maximum item Hit Points must be greater than zero." );
        }

        if ( ( brokenThreshold <= 0 ) || ( brokenThreshold > maximumHitPoints ) )
        {
            throw new ItemCatalogException(
                "Broken Threshold must be positive and cannot exceed maximum item Hit Points." );
        }

        return new DurabilityComponent
        {
            Hardness = hardness,
            MaximumHitPoints = maximumHitPoints,
            BrokenThreshold = brokenThreshold,
        };
    }
}
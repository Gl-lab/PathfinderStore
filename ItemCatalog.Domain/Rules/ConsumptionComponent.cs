using Pathfinder.ItemCatalog.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.ItemCatalog.Domain.Rules;

public sealed class ConsumptionComponent : Entity
{
    private ConsumptionComponent()
    {
    }

    public int ItemRevisionId { get; private set; }
    public ConsumptionMode Mode { get; private set; }
    public int Quantity { get; private set; }

    public static ConsumptionComponent Create( ConsumptionMode mode, int quantity )
    {
        if ( !Enum.IsDefined( mode ) )
        {
            throw new ItemCatalogException( "Consumption mode is invalid." );
        }

        if ( quantity <= 0 )
        {
            throw new ItemCatalogException( "Consumption quantity must be greater than zero." );
        }

        return new ConsumptionComponent
        {
            Mode = mode,
            Quantity = quantity,
        };
    }
}
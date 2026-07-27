using Pathfinder.Commerce.Domain.Exceptions;

namespace Pathfinder.Commerce.Domain.Restocking;

public sealed record RestockSelectionWeights
{
    public RestockSelectionWeights(
        int consumable,
        int permanent,
        int unique )
    {
        if ( consumable < 0 || permanent < 0 || unique < 0 )
        {
            throw new CommerceException( "Restock selection weights cannot be negative." );
        }

        if ( consumable == 0 && permanent == 0 && unique == 0 )
        {
            throw new CommerceException( "At least one restock selection weight must be positive." );
        }

        Consumable = consumable;
        Permanent = permanent;
        Unique = unique;
    }

    public int Consumable { get; }
    public int Permanent { get; }
    public int Unique { get; }

    public int For( RestockItemKind kind ) => kind switch
    {
        RestockItemKind.Consumable => Consumable,
        RestockItemKind.Permanent => Permanent,
        RestockItemKind.Unique => Unique,
        _ => 0,
    };
}

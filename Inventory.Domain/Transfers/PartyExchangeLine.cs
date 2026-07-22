using Pathfinder.Inventory.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Inventory.Domain.Transfers;

public sealed class PartyExchangeLine : Entity
{
    private PartyExchangeLine()
    {
    }

    public int PartyExchangeId { get; private set; }
    public int FromCharacterId { get; private set; }
    public Guid ItemInstanceKey { get; private set; }
    public int ExpectedItemVersion { get; private set; }

    internal static PartyExchangeLine Create(
        int fromCharacterId,
        Guid itemInstanceKey,
        int expectedItemVersion )
    {
        if ( ( fromCharacterId <= 0 ) || ( itemInstanceKey == Guid.Empty ) )
        {
            throw new InventoryException( "Exchange line character id and item key are required." );
        }

        if ( expectedItemVersion < 0 )
        {
            throw new InventoryException( "Exchange line item version cannot be negative." );
        }

        return new PartyExchangeLine
        {
            FromCharacterId = fromCharacterId,
            ItemInstanceKey = itemInstanceKey,
            ExpectedItemVersion = expectedItemVersion,
        };
    }
}

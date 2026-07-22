namespace Pathfinder.Inventory.Domain.Exceptions;

public sealed class InventoryException : Exception
{
    public InventoryException( string message )
        : base( message )
    {
    }
}

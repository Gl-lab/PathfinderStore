namespace Pathfinder.ItemCatalog.Domain.Exceptions;

public sealed class ItemCatalogException : Exception
{
    public ItemCatalogException( string message )
        : base( message )
    {
    }
}
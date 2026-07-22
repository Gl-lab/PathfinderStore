namespace Pathfinder.ItemCatalog.Application.Exceptions;

public sealed class ItemCatalogApplicationException : Exception
{
    public ItemCatalogApplicationException( string message )
        : base( message )
    {
    }
}
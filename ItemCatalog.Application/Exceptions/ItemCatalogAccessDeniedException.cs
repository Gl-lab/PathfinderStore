namespace Pathfinder.ItemCatalog.Application.Exceptions;

public sealed class ItemCatalogAccessDeniedException : Exception
{
    public ItemCatalogAccessDeniedException( string message )
        : base( message )
    {
    }
}
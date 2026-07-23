namespace Pathfinder.Commerce.Domain.Exceptions;

public sealed class CommerceException : Exception
{
    public CommerceException( string message )
        : base( message )
    {
    }
}

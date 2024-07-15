using Domain.Contracts;

namespace Authorization.Exceptions;

public class SecureException: PathfinderException
{
    public SecureException( string message ) : base( message )
    {
        
    }
}
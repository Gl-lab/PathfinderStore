using Pathfinder.Contracts;

namespace Pathfinder.Secure.Domain.Exceptions;

public class SecureException: PathfinderException
{
    public SecureException( string message ) : base( message )
    {
        
    }
}
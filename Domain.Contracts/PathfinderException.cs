namespace Pathfinder.Contracts;

public class PathfinderException : Exception
{
    protected PathfinderException() : base()
    {
        
    }

    public PathfinderException( string message ) : base( message )
    {
        
    }
}
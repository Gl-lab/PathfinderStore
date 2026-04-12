using Pathfinder.Contracts;

namespace Pathfinder.CharacterManagement.Application.Exceptions;

public class CharacterManagementException: PathfinderException 
{
    public CharacterManagementException( string message ) : base( message )
    {
        
    }
}
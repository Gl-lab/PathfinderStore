namespace Pathfinder.CharacterManagement.Domain.Exceptions;

public class CharacterManagementException : Exception
{
    public CharacterManagementException()
    {
    }

    public CharacterManagementException( string message )
        : base( message )
    {
    }

    public CharacterManagementException( string message, Exception innerException )
        : base( message, innerException )
    {
    }
}
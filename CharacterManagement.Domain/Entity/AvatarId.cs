using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Entity;

public sealed record AvatarId
{
    public const int MaxLength = 200;

    private AvatarId( string value )
    {
        Value = value;
    }

    public string Value { get; }

    public static AvatarId Create( string value )
    {
        if ( String.IsNullOrWhiteSpace( value ) )
        {
            throw new CharacterManagementException( "AvatarId cannot be empty." );
        }

        string normalizedValue = value.Trim();
        if ( normalizedValue.Length > MaxLength )
        {
            throw new CharacterManagementException( $"AvatarId cannot exceed {MaxLength} characters." );
        }

        return new AvatarId( normalizedValue );
    }

    public override string ToString() => Value;
}

public static class AvatarIds
{
    public static readonly AvatarId Unknown = AvatarId.Create( "avatar.system.unknown" );
}

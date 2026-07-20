namespace Pathfinder.CharacterManagement.Application.Avatars;

public interface IAvatarSelectionIndexProvider
{
    int Next( int exclusiveUpperBound );
}

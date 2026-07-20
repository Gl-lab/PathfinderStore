namespace Pathfinder.CharacterManagement.Application.Avatars;

public sealed class RandomAvatarSelectionIndexProvider : IAvatarSelectionIndexProvider
{
    public int Next( int exclusiveUpperBound ) => Random.Shared.Next( exclusiveUpperBound );
}

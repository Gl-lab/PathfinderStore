using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Avatars;

public interface IAvatarCatalog
{
    IReadOnlyList<AvatarDefinition> FindMatches( AvatarSelectionCriteria criteria );
    string ResolvePath( AvatarId avatarId );
}

using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Avatars;

public interface IAvatarSelector
{
    AvatarId Select( AvatarSelectionCriteria criteria );
}

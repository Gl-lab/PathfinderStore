using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Avatars;

public sealed class AvatarSelector : IAvatarSelector
{
    private readonly IAvatarCatalog _avatarCatalog;
    private readonly IAvatarSelectionIndexProvider _indexProvider;

    public AvatarSelector(
        IAvatarCatalog avatarCatalog,
        IAvatarSelectionIndexProvider indexProvider )
    {
        _avatarCatalog = avatarCatalog;
        _indexProvider = indexProvider;
    }

    public AvatarId Select( AvatarSelectionCriteria criteria )
    {
        ArgumentNullException.ThrowIfNull( criteria );

        if ( ( criteria.Gender != CharacterGender.Male ) &&
             ( criteria.Gender != CharacterGender.Female ) )
        {
            return AvatarIds.Unknown;
        }

        try
        {
            IReadOnlyList<AvatarDefinition> matches = _avatarCatalog.FindMatches( criteria );
            if ( matches.Count == 0 )
            {
                return AvatarIds.Unknown;
            }

            int selectedIndex = _indexProvider.Next( matches.Count );
            if ( ( selectedIndex < 0 ) || ( selectedIndex >= matches.Count ) )
            {
                return AvatarIds.Unknown;
            }

            return matches[ selectedIndex ].Id;
        }
        catch ( Exception )
        {
            return AvatarIds.Unknown;
        }
    }
}

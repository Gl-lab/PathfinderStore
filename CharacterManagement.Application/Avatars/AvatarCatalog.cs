using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Avatars;

public sealed class AvatarCatalog : IAvatarCatalog
{
    public const string UnknownPath = "/avatars/system/unknown.webp";

    private static readonly IReadOnlyList<AvatarDefinition> DefaultAvatars =
    [
        new AvatarDefinition( AvatarIds.Unknown, UnknownPath ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000001" ),
            "/avatars/pc/000001.webp",
            AncestryType.Dwarf,
            "class.bard",
            CharacterGender.Male,
            Variant: 1 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000002" ),
            "/avatars/pc/000002.webp",
            AncestryType.Dwarf,
            "class.bard",
            CharacterGender.Male,
            Variant: 2 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000003" ),
            "/avatars/pc/000003.webp",
            AncestryType.Dwarf,
            "class.bard",
            CharacterGender.Female,
            Variant: 1 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000004" ),
            "/avatars/pc/000004.webp",
            AncestryType.Dwarf,
            "class.bard",
            CharacterGender.Female,
            Variant: 2 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000005" ),
            "/avatars/pc/000005.webp",
            AncestryType.Dwarf,
            "class.cleric",
            CharacterGender.Male,
            Variant: 1 )
    ];

    private readonly IReadOnlyList<AvatarDefinition> _avatars;

    public AvatarCatalog()
        : this( DefaultAvatars )
    {
    }

    public AvatarCatalog( IReadOnlyList<AvatarDefinition> avatars )
    {
        ArgumentNullException.ThrowIfNull( avatars );
        _avatars = avatars.ToArray();
    }

    public IReadOnlyList<AvatarDefinition> FindMatches( AvatarSelectionCriteria criteria )
    {
        ArgumentNullException.ThrowIfNull( criteria );

        IReadOnlyList<AvatarDefinition> compatibleAvatars = _avatars
            .Where( avatar =>
                avatar.AncestryType == criteria.AncestryType &&
                avatar.CharacterClassId == criteria.CharacterClassId &&
                avatar.Gender == criteria.Gender &&
                ( avatar.HeritageId is null || avatar.HeritageId == criteria.HeritageId ) &&
                ( avatar.SpecializationId is null || avatar.SpecializationId == criteria.SpecializationId ) &&
                ( avatar.BackgroundId is null || avatar.BackgroundId == criteria.BackgroundId ) )
            .ToArray();

        if ( compatibleAvatars.Count == 0 )
        {
            return [];
        }

        int maximumSpecificity = compatibleAvatars.Max( GetSpecificity );
        return compatibleAvatars
            .Where( avatar => GetSpecificity( avatar ) == maximumSpecificity )
            .ToArray();
    }

    public string ResolvePath( AvatarId avatarId )
    {
        ArgumentNullException.ThrowIfNull( avatarId );

        AvatarDefinition? avatar = _avatars.SingleOrDefault( item => item.Id == avatarId );
        return avatar?.Path ?? UnknownPath;
    }

    private static int GetSpecificity( AvatarDefinition avatar )
    {
        return ( avatar.HeritageId is null ? 0 : 1 ) +
               ( avatar.SpecializationId is null ? 0 : 1 ) +
               ( avatar.BackgroundId is null ? 0 : 1 );
    }
}

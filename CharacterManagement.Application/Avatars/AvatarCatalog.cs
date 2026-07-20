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
            Variant: 1 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000006" ),
            "/avatars/pc/000006.webp",
            AncestryType.Dwarf,
            "class.cleric",
            CharacterGender.Male,
            Variant: 2 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000007" ),
            "/avatars/pc/000007.webp",
            AncestryType.Dwarf,
            "class.cleric",
            CharacterGender.Female,
            Variant: 1 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000008" ),
            "/avatars/pc/000008.webp",
            AncestryType.Dwarf,
            "class.cleric",
            CharacterGender.Female,
            Variant: 2 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000009" ),
            "/avatars/pc/000009.webp",
            AncestryType.Dwarf,
            "class.druid",
            CharacterGender.Male,
            Variant: 1 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000010" ),
            "/avatars/pc/000010.webp",
            AncestryType.Dwarf,
            "class.druid",
            CharacterGender.Male,
            Variant: 2 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000011" ),
            "/avatars/pc/000011.webp",
            AncestryType.Dwarf,
            "class.druid",
            CharacterGender.Female,
            Variant: 1 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000012" ),
            "/avatars/pc/000012.webp",
            AncestryType.Dwarf,
            "class.druid",
            CharacterGender.Female,
            Variant: 2 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000013" ),
            "/avatars/pc/000013.webp",
            AncestryType.Dwarf,
            "class.fighter",
            CharacterGender.Male,
            Variant: 1 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000014" ),
            "/avatars/pc/000014.webp",
            AncestryType.Dwarf,
            "class.fighter",
            CharacterGender.Male,
            Variant: 2 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000015" ),
            "/avatars/pc/000015.webp",
            AncestryType.Dwarf,
            "class.fighter",
            CharacterGender.Female,
            Variant: 1 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000016" ),
            "/avatars/pc/000016.webp",
            AncestryType.Dwarf,
            "class.fighter",
            CharacterGender.Female,
            Variant: 2 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000017" ),
            "/avatars/pc/000017.webp",
            AncestryType.Dwarf,
            "class.ranger",
            CharacterGender.Male,
            Variant: 1 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000018" ),
            "/avatars/pc/000018.webp",
            AncestryType.Dwarf,
            "class.ranger",
            CharacterGender.Male,
            Variant: 2 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000019" ),
            "/avatars/pc/000019.webp",
            AncestryType.Dwarf,
            "class.ranger",
            CharacterGender.Female,
            Variant: 1 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000020" ),
            "/avatars/pc/000020.webp",
            AncestryType.Dwarf,
            "class.ranger",
            CharacterGender.Female,
            Variant: 2 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000021" ),
            "/avatars/pc/000021.webp",
            AncestryType.Dwarf,
            "class.rogue",
            CharacterGender.Male,
            Variant: 1 ),
        new AvatarDefinition(
            AvatarId.Create( "avatar.pc.000022" ),
            "/avatars/pc/000022.webp",
            AncestryType.Dwarf,
            "class.rogue",
            CharacterGender.Male,
            Variant: 2 )
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
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Rules.Feats;

public enum CharacterFeatAcquisitionType
{
    Selected,
    Granted
}

public enum CharacterFeatSourceType
{
    Ancestry,
    Background,
    Class,
    ClassChoice
}

public sealed record CharacterFeat(
    FeatDefinition Definition,
    CharacterFeatAcquisitionType AcquisitionType,
    CharacterFeatSourceType SourceType,
    string SourceId );

public static class CharacterFeatResolver
{
    public static IReadOnlyCollection<CharacterFeat> ResolveAncestryAndBackground(
        DraftCharacter character,
        Background? background,
        IReadOnlyCollection<FeatDefinition> featCatalog )
    {
        ArgumentNullException.ThrowIfNull( character );
        ArgumentNullException.ThrowIfNull( featCatalog );

        Dictionary<string, FeatDefinition> feats = featCatalog
            .ToDictionary( feat => feat.Id, StringComparer.Ordinal );
        List<CharacterFeat> result = [];

        if ( !String.IsNullOrWhiteSpace( character.SelectedAncestryFeatId ) )
        {
            FeatDefinition ancestryFeat = GetFeat(
                feats,
                character.SelectedAncestryFeatId,
                FeatCategory.Ancestry );
            result.Add( new CharacterFeat(
                ancestryFeat,
                CharacterFeatAcquisitionType.Selected,
                CharacterFeatSourceType.Ancestry,
                $"ancestry.{character.AncestryType.ToString().ToLowerInvariant()}" ) );
        }

        string? backgroundFeatId = ResolveBackgroundFeatId( character, background );
        if ( background is not null && !String.IsNullOrWhiteSpace( backgroundFeatId ) )
        {
            FeatDefinition backgroundFeat = GetFeat(
                feats,
                backgroundFeatId,
                FeatCategory.Skill );
            result.Add( new CharacterFeat(
                backgroundFeat,
                CharacterFeatAcquisitionType.Granted,
                CharacterFeatSourceType.Background,
                background.Id ) );
        }

        return result
            .OrderBy( feat => feat.Definition.Category )
            .ThenBy( feat => feat.Definition.Name, StringComparer.Ordinal )
            .ToArray();
    }

    private static string? ResolveBackgroundFeatId(
        DraftCharacter character,
        Background? background )
    {
        if ( !String.IsNullOrWhiteSpace( character.SelectedBackgroundSkillFeatId ) )
        {
            return character.SelectedBackgroundSkillFeatId;
        }

        BackgroundGrantDescriptor? fixedGrant = background?.Grants
            .SingleOrDefault( grant =>
                grant.Kind == BackgroundGrantKind.SkillFeat &&
                !grant.RequiresChoice );
        return fixedGrant?.TargetId;
    }

    private static FeatDefinition GetFeat(
        IReadOnlyDictionary<string, FeatDefinition> feats,
        string featId,
        FeatCategory expectedCategory )
    {
        if ( !feats.TryGetValue( featId, out FeatDefinition? feat ) )
        {
            throw new CharacterManagementException( $"Feat '{featId}' is not defined in the feat catalog." );
        }

        if ( feat.Category != expectedCategory )
        {
            throw new CharacterManagementException(
                $"Feat '{featId}' must have category '{expectedCategory}'." );
        }

        return feat;
    }
}

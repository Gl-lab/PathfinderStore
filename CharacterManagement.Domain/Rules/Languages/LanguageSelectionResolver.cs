using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Domain.Rules.Languages;

public sealed record LanguageSelectionOptions(
    AncestryType AncestryType,
    int RequiredCount,
    IReadOnlyList<LanguageId> AvailableLanguageIds );

public sealed class LanguageSelectionResult
{
    public AncestryType AncestryType { get; }
    public int RequiredCount { get; }
    public IReadOnlyList<LanguageId> AvailableLanguageIds { get; }
    public IReadOnlyList<LanguageId> SelectedLanguageIds { get; }

    internal LanguageSelectionResult(
        AncestryType ancestryType,
        int requiredCount,
        IReadOnlyList<LanguageId> availableLanguageIds,
        IReadOnlyList<LanguageId> selectedLanguageIds )
    {
        AncestryType = ancestryType;
        RequiredCount = requiredCount;
        AvailableLanguageIds = availableLanguageIds;
        SelectedLanguageIds = selectedLanguageIds;
    }
}

public static class LanguageSelectionResolver
{
    public static LanguageSelectionOptions ResolveOptions(
        Ancestry ancestry,
        int intelligenceModifier,
        IReadOnlyCollection<LanguageDefinition> languageCatalog,
        IReadOnlyCollection<LanguageId>? accessibleLanguageIds = null )
    {
        ArgumentNullException.ThrowIfNull( ancestry );
        ArgumentNullException.ThrowIfNull( languageCatalog );

        AdditionalLanguageRule rule = ancestry.AdditionalLanguageRule
            ?? throw new CharacterManagementException(
                $"Ancestry '{ancestry.AncestryType}' does not define an additional language rule." );
        Dictionary<string, LanguageDefinition> languagesById = languageCatalog
            .ToDictionary( language => language.Id.Value, StringComparer.Ordinal );

        ValidateReferences( ancestry.StartingLanguages, languagesById, "starting" );
        ValidateReferences( rule.AllowedLanguageIds, languagesById, "allowed" );

        HashSet<LanguageId> availableLanguageIds = rule.AllowedLanguageIds.ToHashSet();
        if ( rule.AllowsCommonLanguages )
        {
            availableLanguageIds.UnionWith(
                languageCatalog
                    .Where( language => language.Rarity == LanguageRarity.Common )
                    .Select( language => language.Id ) );
        }

        if ( rule.AllowsAccessLanguages && accessibleLanguageIds is not null )
        {
            ValidateReferences( accessibleLanguageIds, languagesById, "accessible" );
            availableLanguageIds.UnionWith( accessibleLanguageIds );
        }

        availableLanguageIds.ExceptWith( ancestry.StartingLanguages );

        int requiredCount = rule.Type switch
        {
            AdditionalLanguageRuleType.IntelligenceModifier => Math.Max( 0, intelligenceModifier ),
            AdditionalLanguageRuleType.OnePlusIntelligenceModifier => 1 + Math.Max( 0, intelligenceModifier ),
            _ => throw new CharacterManagementException(
                $"Unsupported additional language rule '{rule.Type}'." ),
        };
        IReadOnlyList<LanguageId> sortedAvailableLanguageIds = availableLanguageIds
            .OrderBy( languageId => languagesById[ languageId.Value ].Name )
            .ToArray();

        if ( sortedAvailableLanguageIds.Count < requiredCount )
        {
            throw new CharacterManagementException(
                $"Ancestry '{ancestry.AncestryType}' offers {sortedAvailableLanguageIds.Count} languages for {requiredCount} required choices." );
        }

        return new LanguageSelectionOptions(
            ancestry.AncestryType,
            requiredCount,
            sortedAvailableLanguageIds );
    }

    public static LanguageSelectionResult ResolveSelection(
        Ancestry ancestry,
        int intelligenceModifier,
        IReadOnlyCollection<LanguageDefinition> languageCatalog,
        IReadOnlyList<string> selectedLanguageIds,
        IReadOnlyCollection<LanguageId>? accessibleLanguageIds = null )
    {
        ArgumentNullException.ThrowIfNull( selectedLanguageIds );

        LanguageSelectionOptions options = ResolveOptions(
            ancestry,
            intelligenceModifier,
            languageCatalog,
            accessibleLanguageIds );

        if ( selectedLanguageIds.Count != options.RequiredCount )
        {
            throw new CharacterManagementException(
                $"Expected {options.RequiredCount} additional languages, got {selectedLanguageIds.Count}." );
        }

        if ( selectedLanguageIds.Any( String.IsNullOrWhiteSpace ) )
        {
            throw new CharacterManagementException( "Additional language ids cannot be empty." );
        }

        if ( selectedLanguageIds.Distinct( StringComparer.Ordinal ).Count() != selectedLanguageIds.Count )
        {
            throw new CharacterManagementException( "Additional languages must be unique." );
        }

        HashSet<string> availableLanguageIds = options.AvailableLanguageIds
            .Select( languageId => languageId.Value )
            .ToHashSet( StringComparer.Ordinal );
        IReadOnlyList<string> unavailableLanguageIds = selectedLanguageIds
            .Where( languageId => !availableLanguageIds.Contains( languageId ) )
            .ToArray();
        if ( unavailableLanguageIds.Count > 0 )
        {
            throw new CharacterManagementException(
                $"Languages are not available for {ancestry.AncestryType}: {String.Join( ", ", unavailableLanguageIds )}." );
        }

        return new LanguageSelectionResult(
            options.AncestryType,
            options.RequiredCount,
            options.AvailableLanguageIds,
            selectedLanguageIds
                .Select( languageId => new LanguageId( languageId ) )
                .ToArray() );
    }

    private static void ValidateReferences(
        IEnumerable<LanguageId> languageIds,
        IReadOnlyDictionary<string, LanguageDefinition> languagesById,
        string referenceKind )
    {
        IReadOnlyList<string> unknownLanguageIds = languageIds
            .Select( languageId => languageId.Value )
            .Where( languageId => !languagesById.ContainsKey( languageId ) )
            .ToArray();
        if ( unknownLanguageIds.Count > 0 )
        {
            throw new CharacterManagementException(
                $"Unknown {referenceKind} language ids: {String.Join( ", ", unknownLanguageIds )}." );
        }
    }
}

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
    private static readonly HashSet<string> SpellshapeFeatIds =
    [
        "feat.reach_spell",
        "feat.widen_spell",
    ];

    public static IReadOnlyCollection<CharacterFeat> Resolve(
        DraftCharacter character,
        Background? background,
        CharacterClass? characterClass,
        BardMuse? bardMuse,
        DruidicOrder? druidicOrder,
        ClericDoctrine? clericDoctrine,
        ArcaneSchool? arcaneSchool,
        ArcaneThesis? arcaneThesis,
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

        if ( characterClass is not null )
        {
            IReadOnlyList<FeatChoiceSlot> slots = GetRequiredClassChoiceSlots(
                characterClass,
                arcaneSchool,
                arcaneThesis );
            if ( character.SelectedClassFeatChoices.Count > 0 )
            {
                ValidateChoices( character.SelectedClassFeatChoices, slots, feats, characterClass );
            }

            foreach ( FeatChoice choice in character.SelectedClassFeatChoices )
            {
                FeatChoiceSlot slot = slots.Single( item => item.SourceId == choice.SourceId );
                FeatDefinition feat = GetFeat( feats, choice.FeatId, slot.Category );
                result.Add( new CharacterFeat(
                    feat,
                    CharacterFeatAcquisitionType.Selected,
                    slot.SourceType,
                    choice.SourceId ) );
            }

            AddClassGrant( result, feats, bardMuse?.Benefits
                .SingleOrDefault( benefit => benefit.Kind == BardMuseBenefitKind.ClassFeat )?.Id,
                bardMuse?.Id );
            AddClassGrant( result, feats, druidicOrder?.Benefits
                .SingleOrDefault( benefit => benefit.Kind == DruidicOrderBenefitKind.ClassFeat )?.Id,
                druidicOrder?.Id );

            if ( clericDoctrine?.Id == "cleric_doctrine.cloistered" )
            {
                AddClassGrant( result, feats, "feat.domain_initiate", clericDoctrine.Id );
            }

            if ( arcaneThesis?.Id == "arcane_thesis.improved_familiar_attunement" )
            {
                AddClassGrant( result, feats, "feat.familiar", arcaneThesis.Id );
            }
        }

        if ( result.Select( feat => feat.Definition.Id ).Distinct( StringComparer.Ordinal ).Count() != result.Count )
        {
            throw new CharacterManagementException( "A feat cannot be selected or granted more than once." );
        }

        return result
            .OrderBy( feat => feat.Definition.Category )
            .ThenBy( feat => feat.Definition.Name, StringComparer.Ordinal )
            .ToArray();
    }

    public static IReadOnlyList<FeatChoice> ResolveClassChoices(
        CharacterClass characterClass,
        ArcaneSchool? arcaneSchool,
        ArcaneThesis? arcaneThesis,
        IReadOnlyList<FeatChoice> choices,
        IReadOnlyCollection<FeatDefinition> featCatalog,
        IReadOnlyCollection<string>? grantedFeatIds = null )
    {
        ArgumentNullException.ThrowIfNull( characterClass );
        ArgumentNullException.ThrowIfNull( choices );
        ArgumentNullException.ThrowIfNull( featCatalog );

        Dictionary<string, FeatDefinition> feats = featCatalog
            .ToDictionary( feat => feat.Id, StringComparer.Ordinal );
        IReadOnlyList<FeatChoiceSlot> slots = GetRequiredClassChoiceSlots(
            characterClass,
            arcaneSchool,
            arcaneThesis );
        ValidateChoices( choices, slots, feats, characterClass );
        if ( grantedFeatIds is not null && choices.Any( choice => grantedFeatIds.Contains( choice.FeatId ) ) )
        {
            throw new CharacterManagementException(
                "A granted class feat cannot also be selected in a class feat slot." );
        }
        return choices.ToArray();
    }

    public static void ValidateSelectedChoicePrerequisites(
        DraftCharacter character,
        IReadOnlyCollection<FeatDefinition> featCatalog,
        IReadOnlyCollection<SkillDefinition> skillCatalog )
    {
        ArgumentNullException.ThrowIfNull( character );
        ArgumentNullException.ThrowIfNull( featCatalog );
        ArgumentNullException.ThrowIfNull( skillCatalog );

        IReadOnlySet<string> ownedFeatIds = character.SelectedClassFeatChoices
            .Select( choice => choice.FeatId )
            .Concat( String.IsNullOrWhiteSpace( character.SelectedAncestryFeatId )
                ? []
                : [ character.SelectedAncestryFeatId ] )
            .Concat( String.IsNullOrWhiteSpace( character.SelectedBackgroundSkillFeatId )
                ? []
                : [ character.SelectedBackgroundSkillFeatId ] )
            .ToHashSet( StringComparer.Ordinal );
        IReadOnlyDictionary<string, FeatDefinition> feats = featCatalog
            .ToDictionary( feat => feat.Id, StringComparer.Ordinal );

        foreach ( FeatChoice choice in character.SelectedClassFeatChoices )
        {
            FeatDefinition feat = feats[ choice.FeatId ];
            foreach ( string prerequisite in feat.Prerequisites )
            {
                if ( prerequisite.StartsWith( "Trained in ", StringComparison.OrdinalIgnoreCase ) )
                {
                    string skillRequirement = prerequisite[ "Trained in ".Length.. ].TrimEnd( '.' );
                    string[] skillNames = skillRequirement.Split(
                        " or ",
                        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );
                    SkillDefinition[] skills = skillCatalog
                        .Where( item => skillNames.Contains( item.Name, StringComparer.OrdinalIgnoreCase ) )
                        .ToArray();
                    if ( skills.Length > 0 && !skills.Any( skill =>
                            character.TrainedSkills.Any( training => training.SkillId == skill.Id ) ) )
                    {
                        throw new CharacterManagementException(
                            $"Feat '{feat.Id}' requires training in '{skillRequirement}'." );
                    }
                    continue;
                }

                string prerequisiteName = prerequisite.TrimEnd( '.' );
                FeatDefinition? prerequisiteFeat = featCatalog.SingleOrDefault(
                    item => item.Name.Equals( prerequisiteName, StringComparison.OrdinalIgnoreCase ) );
                if ( prerequisiteFeat is not null && !ownedFeatIds.Contains( prerequisiteFeat.Id ) )
                {
                    throw new CharacterManagementException(
                        $"Feat '{feat.Id}' requires feat '{prerequisiteFeat.Id}'." );
                }
            }
        }
    }

    private static IReadOnlyList<FeatChoiceSlot> GetRequiredClassChoiceSlots(
        CharacterClass characterClass,
        ArcaneSchool? arcaneSchool,
        ArcaneThesis? arcaneThesis )
    {
        List<FeatChoiceSlot> slots = characterClass.Rules
            .Where( rule => rule.Kind == CharacterClassRuleKind.ClassFeatChoice )
            .Select( rule => new FeatChoiceSlot(
                rule.Id,
                FeatCategory.Class,
                CharacterFeatSourceType.Class,
                false ) )
            .ToList();
        slots.AddRange( characterClass.Rules
            .Where( rule => rule.Kind == CharacterClassRuleKind.SkillFeatChoice )
            .Select( rule => new FeatChoiceSlot(
                rule.Id,
                FeatCategory.Skill,
                CharacterFeatSourceType.Class,
                false ) ) );

        ArcaneThesisEffectDescriptor? spellshapeChoice = arcaneThesis?.Effects
            .SingleOrDefault( effect => effect.Kind == ArcaneThesisEffectKind.FirstLevelSpellshapeFeatChoice );
        if ( spellshapeChoice is not null )
        {
            slots.Add( new FeatChoiceSlot(
                spellshapeChoice.Id,
                FeatCategory.Class,
                CharacterFeatSourceType.ClassChoice,
                true ) );
        }

        ArcaneSchoolBenefitDescriptor? extraClassFeat = arcaneSchool?.Benefits
            .SingleOrDefault( benefit => benefit.Kind == ArcaneSchoolBenefitKind.ExtraClassFeat );
        if ( extraClassFeat is not null )
        {
            slots.Add( new FeatChoiceSlot(
                extraClassFeat.Id,
                FeatCategory.Class,
                CharacterFeatSourceType.ClassChoice,
                false ) );
        }

        return slots;
    }

    private static void ValidateChoices(
        IReadOnlyList<FeatChoice> choices,
        IReadOnlyList<FeatChoiceSlot> slots,
        IReadOnlyDictionary<string, FeatDefinition> feats,
        CharacterClass characterClass )
    {
        if ( choices.Count != slots.Count )
        {
            throw new CharacterManagementException(
                $"Class package requires {slots.Count} class feat choice(s), got {choices.Count}." );
        }

        if ( choices.Select( choice => choice.SourceId ).Distinct( StringComparer.Ordinal ).Count() != choices.Count )
        {
            throw new CharacterManagementException( "Class feat choice sources must be unique." );
        }

        if ( choices.Select( choice => choice.FeatId ).Distinct( StringComparer.Ordinal ).Count() != choices.Count )
        {
            throw new CharacterManagementException( "Class feat choices must be unique." );
        }

        string classTrait = characterClass.Name;
        foreach ( FeatChoiceSlot slot in slots )
        {
            FeatChoice? choice = choices.SingleOrDefault( item => item.SourceId == slot.SourceId );
            if ( choice is null )
            {
                throw new CharacterManagementException( $"Feat choice '{slot.SourceId}' is required." );
            }

            FeatDefinition feat = GetFeat( feats, choice.FeatId, slot.Category );
            bool hasRequiredClassTrait = slot.Category != FeatCategory.Class ||
                                         feat.Traits.Contains( classTrait, StringComparer.OrdinalIgnoreCase );
            if ( feat.Level != 1 || !hasRequiredClassTrait )
            {
                throw new CharacterManagementException(
                    $"Feat '{feat.Id}' is not available for choice '{slot.SourceId}'." );
            }

            if ( slot.Category == FeatCategory.Skill &&
                 feat.DeferredDependencies.Contains( FeatDependencyType.FeatParameterChoice ) )
            {
                throw new CharacterManagementException(
                    $"Feat '{feat.Id}' requires an unsupported parameter choice." );
            }

            if ( slot.RequiresSpellshape && !SpellshapeFeatIds.Contains( feat.Id ) )
            {
                throw new CharacterManagementException(
                    $"Feat choice '{slot.SourceId}' requires a 1st-level spellshape feat." );
            }
        }
    }

    private static void AddClassGrant(
        ICollection<CharacterFeat> result,
        IReadOnlyDictionary<string, FeatDefinition> feats,
        string? featId,
        string? sourceId )
    {
        if ( String.IsNullOrWhiteSpace( featId ) || String.IsNullOrWhiteSpace( sourceId ) )
        {
            return;
        }

        FeatDefinition feat = GetFeat( feats, featId, FeatCategory.Class );
        result.Add( new CharacterFeat(
            feat,
            CharacterFeatAcquisitionType.Granted,
            CharacterFeatSourceType.ClassChoice,
            sourceId ) );
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

    private sealed record FeatChoiceSlot(
        string SourceId,
        FeatCategory Category,
        CharacterFeatSourceType SourceType,
        bool RequiresSpellshape );
}

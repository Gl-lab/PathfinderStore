using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class FeatRepository : IFeatRepository
{
    private readonly IReadOnlyDictionary<string, FeatDefinition> _feats;

    public FeatRepository(
        IAncestryRepository ancestryRepository,
        IBackgroundRepository backgroundRepository )
    {
        ArgumentNullException.ThrowIfNull( ancestryRepository );
        ArgumentNullException.ThrowIfNull( backgroundRepository );

        IReadOnlyCollection<FeatDefinition> feats =
        [
            .. CreateAncestryFeats( ancestryRepository ),
            .. CreateBackgroundSkillFeats( backgroundRepository ),
            .. CreateClassFeats(),
        ];

        string[] duplicateIds = feats
            .GroupBy( feat => feat.Id, StringComparer.Ordinal )
            .Where( group => group.Count() > 1 )
            .Select( group => group.Key )
            .ToArray();

        if ( duplicateIds.Length > 0 )
        {
            throw new InvalidOperationException(
                $"Feat ids must be unique: {String.Join( ", ", duplicateIds )}." );
        }

        _feats = feats.ToDictionary( feat => feat.Id, StringComparer.Ordinal );
    }

    public IReadOnlyCollection<FeatDefinition> GetAll() => _feats.Values
        .OrderBy( feat => feat.Category )
        .ThenBy( feat => feat.Level )
        .ThenBy( feat => feat.Name, StringComparer.Ordinal )
        .ToArray();

    public FeatDefinition GetFeat( string featId )
    {
        if ( String.IsNullOrWhiteSpace( featId ) )
        {
            throw new ArgumentException( "Feat id cannot be empty.", nameof( featId ) );
        }

        if ( !_feats.TryGetValue( featId, out FeatDefinition? feat ) )
        {
            throw new ArgumentOutOfRangeException( nameof( featId ), $"Feat '{featId}' is not defined." );
        }

        return feat;
    }

    private static IReadOnlyCollection<FeatDefinition> CreateAncestryFeats(
        IAncestryRepository ancestryRepository )
    {
        return ancestryRepository
            .GetAll()
            .SelectMany( ancestry => ancestry.AncestryFeats )
            .Select( feat => new FeatDefinition(
                feat.Id,
                feat.Name,
                FeatCategory.Ancestry,
                feat.Level,
                [ feat.AncestryType.ToString() ],
                feat.Rarity == AncestryChoiceRarity.Common ? FeatRarity.Common : FeatRarity.Uncommon,
                feat.Prerequisites,
                String.Join( " ", feat.Effects.Select( effect => effect.Summary ) ),
                feat.DeferredDependencies
                    .Select( MapDependency )
                    .ToArray(),
                feat.Source ) )
            .ToArray();
    }

    private static IReadOnlyCollection<FeatDefinition> CreateBackgroundSkillFeats(
        IBackgroundRepository backgroundRepository )
    {
        return backgroundRepository
            .GetAll()
            .SelectMany( background => background.Grants
                .Where( grant => grant.Kind == BackgroundGrantKind.SkillFeat )
                .SelectMany( grant => GetSkillFeatTargets( background, grant ) ) )
            .GroupBy( target => target.Id, StringComparer.Ordinal )
            .Select( group => group.First() )
            .Select( target => new FeatDefinition(
                target.Id,
                target.Name,
                FeatCategory.Skill,
                1,
                [ "General", "Skill" ],
                FeatRarity.Common,
                [],
                target.Summary,
                [ FeatDependencyType.RuleEngine, FeatDependencyType.SkillCatalog ],
                target.Source ) )
            .ToArray();
    }

    private static IReadOnlyCollection<BackgroundSkillFeatTarget> GetSkillFeatTargets(
        Background background,
        BackgroundGrantDescriptor grant )
    {
        if ( grant.RequiresChoice )
        {
            return grant.Options
                .Select( option => new BackgroundSkillFeatTarget(
                    option.Id,
                    option.Name,
                    grant.Summary,
                    background.Source ) )
                .ToArray();
        }

        return
        [
            new BackgroundSkillFeatTarget(
                grant.TargetId ?? throw new InvalidOperationException( $"Background grant '{grant.Id}' has no target." ),
                grant.Name,
                grant.Summary,
                background.Source ),
        ];
    }

    private static IReadOnlyCollection<FeatDefinition> CreateClassFeats()
    {
        return
        [
            ClassFeat( "feat.bardic_lore", "Bardic Lore", 100, "Bard", "enigma muse", FeatDependencyType.SkillCatalog, FeatDependencyType.LoreCatalog ),
            ClassFeat( "feat.hymn_of_healing", "Hymn of Healing", 101, "Bard", dependencies: [ FeatDependencyType.FocusSpellRules ] ),
            ClassFeat( "feat.lingering_composition", "Lingering Composition", 101, "Bard", "maestro muse", FeatDependencyType.FocusSpellRules ),
            ClassFeat( "feat.martial_performance", "Martial Performance", 101, "Bard", "warrior muse", FeatDependencyType.WeaponCatalog, FeatDependencyType.ProficiencyRules ),
            ClassFeat( "feat.versatile_performance", "Versatile Performance", 101, "Bard", "polymath muse", FeatDependencyType.SkillCatalog ),
            ClassFeat( "feat.well_versed", "Well-Versed", 101, "Bard", dependencies: [ FeatDependencyType.ConditionRules ] ),
            ClassFeat( "feat.deadly_simplicity", "Deadly Simplicity", 113, "Cleric", "Deity with a simple or unarmed favored weapon; trained with that weapon.", FeatDependencyType.WeaponCatalog, FeatDependencyType.CombatRules ),
            ClassFeat( "feat.divine_castigation", "Divine Castigation", 113, "Cleric", "Holy or unholy sanctification.", FeatDependencyType.Spellcasting, FeatDependencyType.CombatRules ),
            ClassFeat( "feat.domain_initiate", "Domain Initiate", 113, "Cleric", dependencies: [ FeatDependencyType.FocusSpellRules ] ),
            ClassFeat( "feat.harming_hands", "Harming Hands", 114, "Cleric", "Harmful font.", FeatDependencyType.Spellcasting, FeatDependencyType.CombatRules ),
            ClassFeat( "feat.healing_hands", "Healing Hands", 114, "Cleric", "Healing font.", FeatDependencyType.Spellcasting, FeatDependencyType.CombatRules ),
            ClassFeat( "feat.premonition_of_avoidance", "Premonition of Avoidance", 114, "Cleric", dependencies: [ FeatDependencyType.RuleEngine ] ),
            ClassFeat( "feat.animal_companion", "Animal Companion", 127, "Druid|Ranger", "Druid: animal order.", FeatDependencyType.AnimalCompanionRules ),
            ClassFeat( "feat.animal_empathy", "Animal Empathy", 127, "Druid", dependencies: [ FeatDependencyType.SkillCatalog, FeatDependencyType.RuleEngine ] ),
            ClassFeat( "feat.leshy_familiar", "Leshy Familiar", 127, "Druid", "Leaf order.", FeatDependencyType.FamiliarRules ),
            ClassFeat( "feat.plant_empathy", "Plant Empathy", 127, "Druid", dependencies: [ FeatDependencyType.SkillCatalog, FeatDependencyType.RuleEngine ] ),
            ClassFeat( "feat.storm_born", "Storm Born", 128, "Druid", "Storm order.", FeatDependencyType.CombatRules ),
            ClassFeat( "feat.untamed_form", "Untamed Form", 128, "Druid", "Untamed order.", FeatDependencyType.FocusSpellRules ),
            ClassFeat( "feat.verdant_weapon", "Verdant Weapon", 128, "Druid", dependencies: [ FeatDependencyType.WeaponCatalog, FeatDependencyType.InventoryCatalog ] ),
            ClassFeat( "feat.combat_assessment", "Combat Assessment", 140, "Fighter", dependencies: [ FeatDependencyType.SkillCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.double_slice", "Double Slice", 140, "Fighter", dependencies: [ FeatDependencyType.WeaponCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.exacting_strike", "Exacting Strike", 140, "Fighter", dependencies: [ FeatDependencyType.WeaponCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.point_blank_stance", "Point Blank Stance", 140, "Fighter", dependencies: [ FeatDependencyType.WeaponCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.reactive_shield", "Reactive Shield", 140, "Fighter", dependencies: [ FeatDependencyType.InventoryCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.snagging_strike", "Snagging Strike", 141, "Fighter", dependencies: [ FeatDependencyType.WeaponCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.sudden_charge", "Sudden Charge", 141, "Fighter", dependencies: [ FeatDependencyType.MovementRules, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.vicious_swing", "Vicious Swing", 141, "Fighter", dependencies: [ FeatDependencyType.WeaponCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.crossbow_ace", "Crossbow Ace", 157, "Ranger", dependencies: [ FeatDependencyType.WeaponCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.hunted_shot", "Hunted Shot", 157, "Ranger", dependencies: [ FeatDependencyType.WeaponCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.initiate_warden", "Initiate Warden", 157, "Ranger", dependencies: [ FeatDependencyType.FocusSpellRules ] ),
            ClassFeat( "feat.monster_hunter", "Monster Hunter", 157, "Ranger", dependencies: [ FeatDependencyType.SkillCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.twin_takedown", "Twin Takedown", 157, "Ranger", dependencies: [ FeatDependencyType.WeaponCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.nimble_dodge", "Nimble Dodge", 169, "Rogue", dependencies: [ FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.overextending_feint", "Overextending Feint", 169, "Rogue", "Trained in Deception.", FeatDependencyType.SkillCatalog, FeatDependencyType.CombatRules ),
            ClassFeat( "feat.plant_evidence", "Plant Evidence", 169, "Rogue", "Pickpocket.", FeatDependencyType.SkillCatalog, FeatDependencyType.RuleEngine ),
            ClassFeat( "feat.trap_finder", "Trap Finder", 169, "Rogue", dependencies: [ FeatDependencyType.PerceptionRules, FeatDependencyType.SkillCatalog ] ),
            ClassFeat( "feat.tumble_behind", "Tumble Behind", 170, "Rogue", dependencies: [ FeatDependencyType.SkillCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.twin_feint", "Twin Feint", 170, "Rogue", dependencies: [ FeatDependencyType.WeaponCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.you_re_next", "You're Next", 171, "Rogue", "Trained in Intimidation.", FeatDependencyType.SkillCatalog, FeatDependencyType.ConditionRules ),
            ClassFeat( "feat.cackle", "Cackle", 186, "Witch", dependencies: [ FeatDependencyType.FocusSpellRules ] ),
            ClassFeat( "feat.cauldron", "Cauldron", 186, "Witch", dependencies: [ FeatDependencyType.InventoryCatalog, FeatDependencyType.RuleEngine ] ),
            ClassFeat( "feat.counterspell", "Counterspell", 186, "Witch|Wizard", dependencies: [ FeatDependencyType.Spellcasting, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.witch_s_armaments", "Witch's Armaments", 186, "Witch", dependencies: [ FeatDependencyType.WeaponCatalog, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.familiar", "Familiar", 201, "Wizard", dependencies: [ FeatDependencyType.FamiliarRules ] ),
            ClassFeat( "feat.spellbook_prodigy", "Spellbook Prodigy", 201, "Wizard", "Trained in Arcana.", FeatDependencyType.SkillCatalog, FeatDependencyType.SpellCatalog ),
            ClassFeat( "feat.reach_spell", "Reach Spell", 101, "Bard|Cleric|Druid|Witch|Wizard", dependencies: [ FeatDependencyType.Spellcasting, FeatDependencyType.CombatRules ] ),
            ClassFeat( "feat.widen_spell", "Widen Spell", 128, "Druid|Witch|Wizard", dependencies: [ FeatDependencyType.Spellcasting, FeatDependencyType.CombatRules ] ),
        ];
    }

    private static FeatDefinition ClassFeat(
        string id,
        string name,
        int page,
        string classTraits,
        string? prerequisite = null,
        params FeatDependencyType[] dependencies )
    {
        IReadOnlyList<FeatDependencyType> normalizedDependencies = dependencies.Length == 0
            ? [ FeatDependencyType.RuleEngine ]
            : dependencies;

        return new FeatDefinition(
            id,
            name,
            FeatCategory.Class,
            1,
            classTraits.Split( '|', StringSplitOptions.RemoveEmptyEntries ),
            FeatRarity.Common,
            String.IsNullOrWhiteSpace( prerequisite ) ? [] : [ prerequisite ],
            "The feat's mechanical effect is deferred to the listed rule subsystem.",
            normalizedDependencies,
            new SourceReference( "Player Core", page ) );
    }

    private static FeatDependencyType MapDependency( AncestryDependencyType dependency )
    {
        return dependency switch
        {
            AncestryDependencyType.RuleEngine => FeatDependencyType.RuleEngine,
            AncestryDependencyType.SpellCatalog => FeatDependencyType.SpellCatalog,
            AncestryDependencyType.Spellcasting => FeatDependencyType.Spellcasting,
            AncestryDependencyType.ClassCatalog => FeatDependencyType.ClassCatalog,
            AncestryDependencyType.ClassFeatCatalog => FeatDependencyType.ClassFeatCatalog,
            AncestryDependencyType.GeneralFeatCatalog => FeatDependencyType.GeneralFeatCatalog,
            AncestryDependencyType.SkillCatalog => FeatDependencyType.SkillCatalog,
            AncestryDependencyType.LoreCatalog => FeatDependencyType.LoreCatalog,
            AncestryDependencyType.WeaponCatalog => FeatDependencyType.WeaponCatalog,
            AncestryDependencyType.ProficiencyRules => FeatDependencyType.ProficiencyRules,
            AncestryDependencyType.InventoryCatalog => FeatDependencyType.InventoryCatalog,
            AncestryDependencyType.LanguageCatalog => FeatDependencyType.LanguageCatalog,
            AncestryDependencyType.CombatRules => FeatDependencyType.CombatRules,
            AncestryDependencyType.MovementRules => FeatDependencyType.MovementRules,
            AncestryDependencyType.ConditionRules => FeatDependencyType.ConditionRules,
            AncestryDependencyType.ResistanceRules => FeatDependencyType.ResistanceRules,
            AncestryDependencyType.EnvironmentRules => FeatDependencyType.EnvironmentRules,
            AncestryDependencyType.PerceptionRules => FeatDependencyType.PerceptionRules,
            AncestryDependencyType.AnimalCompanionRules => FeatDependencyType.AnimalCompanionRules,
            AncestryDependencyType.ArchetypeCatalog => FeatDependencyType.ArchetypeCatalog,
            _ => throw new ArgumentOutOfRangeException( nameof( dependency ), dependency, null ),
        };
    }

    private sealed record BackgroundSkillFeatTarget(
        string Id,
        string Name,
        string Summary,
        SourceReference Source );
}

using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Infrastructure.Repositories;

public sealed class ArcaneThesisRepository : IArcaneThesisRepository
{
    private static readonly Dictionary<string, ArcaneThesis> ArcaneTheses = CreateArcaneTheses()
        .ToDictionary( thesis => thesis.Id, StringComparer.Ordinal );

    public IReadOnlyCollection<ArcaneThesis> GetAll() => ArcaneTheses.Values.ToArray();

    public ArcaneThesis GetArcaneThesis( string arcaneThesisId )
    {
        if ( String.IsNullOrWhiteSpace( arcaneThesisId ) )
        {
            throw new ArgumentException( "Arcane Thesis id cannot be empty.", nameof( arcaneThesisId ) );
        }

        if ( !ArcaneTheses.TryGetValue( arcaneThesisId, out ArcaneThesis? thesis ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof( arcaneThesisId ),
                $"Arcane Thesis '{arcaneThesisId}' is not defined." );
        }

        return thesis;
    }

    private static IReadOnlyCollection<ArcaneThesis> CreateArcaneTheses()
    {
        return
        [
            Create(
                "experimental_spellshaping",
                "Experimental Spellshaping",
                [
                    Effect(
                        "experimental_spellshaping",
                        "first_level_spellshape_feat_choice",
                        ArcaneThesisEffectKind.FirstLevelSpellshapeFeatChoice,
                        "First-Level Spellshape Feat",
                        "Grants one 1st-level wizard spellshape feat of your choice.",
                        [ 1 ],
                        [] ),
                    Effect(
                        "experimental_spellshaping",
                        "daily_spellshape_feat_choice",
                        ArcaneThesisEffectKind.DailySpellshapeFeatChoice,
                        "Daily Spellshape Feat",
                        "Starting at 4th level, daily preparations grant a temporary spellshape feat up to half your level.",
                        [ 4 ],
                        [
                            CharacterClassDependencyType.ClassFeatCatalog,
                            CharacterClassDependencyType.ClassFeatureRules,
                        ] ),
                ] ),
            Create(
                "improved_familiar_attunement",
                "Improved Familiar Attunement",
                [
                    Effect(
                        "improved_familiar_attunement",
                        "familiar_feat_grant",
                        ArcaneThesisEffectKind.FamiliarFeatGrant,
                        "Familiar Feat",
                        "Grants the Familiar wizard feat.",
                        [ 1 ],
                        [] ),
                    Effect(
                        "improved_familiar_attunement",
                        "familiar_ability_progression",
                        ArcaneThesisEffectKind.FamiliarAbilityProgression,
                        "Familiar Ability Progression",
                        "Grants an extra familiar ability at 1st, 6th, 12th, and 18th levels.",
                        [ 1, 6, 12, 18 ],
                        [ CharacterClassDependencyType.FamiliarRules ] ),
                    Effect(
                        "improved_familiar_attunement",
                        "drain_familiar_replacement",
                        ArcaneThesisEffectKind.DrainFamiliarReplacement,
                        "Drain Familiar",
                        "Uses the familiar as the arcane bond and replaces Drain Bonded Item with Drain Familiar.",
                        [ 1 ],
                        [
                            CharacterClassDependencyType.FamiliarRules,
                            CharacterClassDependencyType.ClassFeatureRules,
                        ] ),
                ] ),
            Create(
                "spell_blending",
                "Spell Blending",
                [
                    Effect(
                        "spell_blending",
                        "spell_slot_blending",
                        ArcaneThesisEffectKind.SpellSlotBlending,
                        "Spell Blending",
                        "Trades spell slots during daily preparations for higher-rank bonus slots or additional cantrips.",
                        [ 1 ],
                        [
                            CharacterClassDependencyType.SpellSlotRules,
                            CharacterClassDependencyType.SpellPreparationRules,
                        ] ),
                ] ),
            Create(
                "spell_substitution",
                "Spell Substitution",
                [
                    Effect(
                        "spell_substitution",
                        "prepared_spell_substitution",
                        ArcaneThesisEffectKind.PreparedSpellSubstitution,
                        "Spell Substitution",
                        "Spends 10 minutes to replace a prepared spell with another spell from the spellbook.",
                        [ 1 ],
                        [
                            CharacterClassDependencyType.SpellCatalog,
                            CharacterClassDependencyType.SpellPreparationRules,
                        ] ),
                ] ),
            Create(
                "staff_nexus",
                "Staff Nexus",
                [
                    Effect(
                        "staff_nexus",
                        "makeshift_staff",
                        ArcaneThesisEffectKind.MakeshiftStaff,
                        "Makeshift Staff",
                        "Begins play with a makeshift staff containing one cantrip and one 1st-rank spell from the spellbook.",
                        [ 1 ],
                        [
                            CharacterClassDependencyType.ItemCatalog,
                            CharacterClassDependencyType.SpellCatalog,
                            CharacterClassDependencyType.SpellPreparationRules,
                        ] ),
                    Effect(
                        "staff_nexus",
                        "staff_charge_preparation",
                        ArcaneThesisEffectKind.StaffChargePreparation,
                        "Staff Charge Preparation",
                        "Expends a spell during daily preparations to add charges and can merge the makeshift staff with another staff.",
                        [ 1 ],
                        [
                            CharacterClassDependencyType.ItemCatalog,
                            CharacterClassDependencyType.SpellSlotRules,
                            CharacterClassDependencyType.SpellPreparationRules,
                            CharacterClassDependencyType.ClassFeatureRules,
                        ] ),
                    Effect(
                        "staff_nexus",
                        "staff_charge_progression",
                        ArcaneThesisEffectKind.StaffChargeProgression,
                        "Staff Charge Progression",
                        "Can expend two spells at 8th level and three spells at 16th level to add staff charges.",
                        [ 8, 16 ],
                        [
                            CharacterClassDependencyType.ItemCatalog,
                            CharacterClassDependencyType.SpellSlotRules,
                            CharacterClassDependencyType.SpellPreparationRules,
                            CharacterClassDependencyType.ClassFeatureRules,
                        ] ),
                ] ),
        ];
    }

    private static ArcaneThesis Create(
        string id,
        string name,
        IReadOnlyList<ArcaneThesisEffectDescriptor> effects )
    {
        return new ArcaneThesis(
            $"arcane_thesis.{id}",
            name,
            new SourceReference( "Player Core", 195 ),
            effects );
    }

    private static ArcaneThesisEffectDescriptor Effect(
        string thesisId,
        string effectId,
        ArcaneThesisEffectKind kind,
        string name,
        string summary,
        IReadOnlyList<int> milestoneLevels,
        IReadOnlyList<CharacterClassDependencyType> dependencies )
    {
        return new ArcaneThesisEffectDescriptor(
            $"arcane_thesis.{thesisId}.effect.{effectId}",
            kind,
            name,
            summary,
            milestoneLevels,
            dependencies );
    }
}

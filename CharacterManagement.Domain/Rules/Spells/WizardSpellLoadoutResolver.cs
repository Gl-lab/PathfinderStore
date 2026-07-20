using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Domain.Rules.Spells;

public static class WizardSpellLoadoutResolver
{
    public static WizardSpellLoadout Resolve(
        ArcaneSchool school,
        IReadOnlyList<string> spellbookCantripIds,
        IReadOnlyList<string> spellbookRankOneSpellIds,
        string? curriculumCantripId,
        IReadOnlyList<string> curriculumRankOneSpellIds,
        IReadOnlyList<string> preparedCantripIds,
        IReadOnlyList<string> preparedRankOneSpellIds,
        string? preparedCurriculumCantripId,
        string? preparedCurriculumRankOneSpellId,
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        ArgumentNullException.ThrowIfNull( school );
        ArgumentNullException.ThrowIfNull( spellbookCantripIds );
        ArgumentNullException.ThrowIfNull( spellbookRankOneSpellIds );
        ArgumentNullException.ThrowIfNull( curriculumRankOneSpellIds );
        ArgumentNullException.ThrowIfNull( preparedCantripIds );
        ArgumentNullException.ThrowIfNull( preparedRankOneSpellIds );
        ArgumentNullException.ThrowIfNull( spellCatalog );

        ValidateUniqueCount( spellbookCantripIds, 10, "A Wizard spellbook must contain exactly 10 unique base cantrips." );
        int baseSpellCount = school.HasCurriculum ? 5 : 6;
        ValidateUniqueCount(
            spellbookRankOneSpellIds,
            baseSpellCount,
            $"A Wizard spellbook must contain exactly {baseSpellCount} unique base rank-1 spells for this school." );
        ValidateUniqueCount( preparedCantripIds, 5, "A first-level Wizard must prepare exactly 5 unique base cantrips." );
        if ( ( preparedRankOneSpellIds.Count != 2 ) || preparedRankOneSpellIds.Any( String.IsNullOrWhiteSpace ) )
        {
            throw new ArgumentException( "A first-level Wizard must prepare exactly 2 base rank-1 spell slots." );
        }

        IReadOnlySet<string> commonCantripIds = SpellCatalogResolver
            .ResolveCommonOptions( spellCatalog, SpellTradition.Arcane, 1, SpellKind.Cantrip )
            .Select( spell => spell.Id )
            .ToHashSet( StringComparer.Ordinal );
        IReadOnlySet<string> commonSpellIds = SpellCatalogResolver
            .ResolveCommonOptions( spellCatalog, SpellTradition.Arcane, 1, SpellKind.Spell )
            .Select( spell => spell.Id )
            .ToHashSet( StringComparer.Ordinal );
        if ( spellbookCantripIds.Any( spellId => !commonCantripIds.Contains( spellId ) ) ||
             spellbookRankOneSpellIds.Any( spellId => !commonSpellIds.Contains( spellId ) ) )
        {
            throw new ArgumentException( "Base Wizard spellbook choices must be common Arcane spells." );
        }

        IReadOnlySet<string> knownCantripIds = spellbookCantripIds.ToHashSet( StringComparer.Ordinal );
        IReadOnlySet<string> knownSpellIds = spellbookRankOneSpellIds.ToHashSet( StringComparer.Ordinal );
        if ( preparedCantripIds.Any( spellId => !knownCantripIds.Contains( spellId ) ) ||
             preparedRankOneSpellIds.Any( spellId => !knownSpellIds.Contains( spellId ) ) )
        {
            throw new ArgumentException( "Prepared Wizard base spells must be present in the spellbook." );
        }

        if ( school.HasCurriculum )
        {
            ValidateFormalSchoolChoices(
                school,
                curriculumCantripId,
                curriculumRankOneSpellIds,
                preparedCurriculumCantripId,
                preparedCurriculumRankOneSpellId,
                spellCatalog );
        }
        else if ( !String.IsNullOrWhiteSpace( curriculumCantripId ) ||
                  ( curriculumRankOneSpellIds.Count > 0 ) ||
                  !String.IsNullOrWhiteSpace( preparedCurriculumCantripId ) ||
                  !String.IsNullOrWhiteSpace( preparedCurriculumRankOneSpellId ) )
        {
            throw new ArgumentException( "Unified Magical Theory cannot select curriculum spells or slots." );
        }

        string initialSchoolSpellId = school.Benefits
            .Single( benefit => benefit.Kind == ArcaneSchoolBenefitKind.InitialSchoolSpell )
            .Id;
        SpellDefinition initialSchoolSpell = ResolveSpell( initialSchoolSpellId, spellCatalog );
        if ( ( initialSchoolSpell.Kind != SpellKind.Focus ) ||
             !initialSchoolSpell.Traditions.Contains( SpellTradition.Arcane ) )
        {
            throw new ArgumentException( "Initial Wizard school spell has invalid metadata.", nameof( spellCatalog ) );
        }

        return new WizardSpellLoadout(
            spellbookCantripIds,
            spellbookRankOneSpellIds,
            curriculumCantripId,
            curriculumRankOneSpellIds,
            preparedCantripIds,
            preparedRankOneSpellIds,
            preparedCurriculumCantripId,
            preparedCurriculumRankOneSpellId,
            initialSchoolSpellId );
    }

    private static void ValidateFormalSchoolChoices(
        ArcaneSchool school,
        string? curriculumCantripId,
        IReadOnlyList<string> curriculumRankOneSpellIds,
        string? preparedCurriculumCantripId,
        string? preparedCurriculumRankOneSpellId,
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        ValidateUniqueCount( curriculumRankOneSpellIds, 2, "A formal-school Wizard must add exactly 2 unique rank-1 curriculum spells." );
        IReadOnlySet<string> curriculumCantripIds = school.CurriculumSpells
            .Where( spell => spell.Rank == 0 )
            .Select( spell => spell.Id )
            .ToHashSet( StringComparer.Ordinal );
        IReadOnlySet<string> curriculumSpellIds = school.CurriculumSpells
            .Where( spell => spell.Rank == 1 )
            .Select( spell => spell.Id )
            .ToHashSet( StringComparer.Ordinal );
        if ( String.IsNullOrWhiteSpace( curriculumCantripId ) ||
             !curriculumCantripIds.Contains( curriculumCantripId ) ||
             curriculumRankOneSpellIds.Any( spellId => !curriculumSpellIds.Contains( spellId ) ) ||
             String.IsNullOrWhiteSpace( preparedCurriculumCantripId ) ||
             !curriculumCantripIds.Contains( preparedCurriculumCantripId ) ||
             String.IsNullOrWhiteSpace( preparedCurriculumRankOneSpellId ) ||
             !curriculumSpellIds.Contains( preparedCurriculumRankOneSpellId ) )
        {
            throw new ArgumentException( "Wizard curriculum choices must belong to the selected Arcane School." );
        }

        string[] selectedIds = curriculumRankOneSpellIds
            .Append( curriculumCantripId )
            .Append( preparedCurriculumCantripId )
            .Append( preparedCurriculumRankOneSpellId )
            .Distinct( StringComparer.Ordinal )
            .ToArray();
        foreach ( string spellId in selectedIds )
        {
            SpellDefinition spell = ResolveSpell( spellId, spellCatalog );
            if ( !spell.Traditions.Contains( SpellTradition.Arcane ) )
            {
                throw new ArgumentException( "Wizard curriculum spell has invalid metadata.", nameof( spellCatalog ) );
            }
        }
    }

    private static void ValidateUniqueCount( IReadOnlyList<string> spellIds, int count, string message )
    {
        if ( ( spellIds.Count != count ) ||
             ( spellIds.Distinct( StringComparer.Ordinal ).Count() != count ) )
        {
            throw new ArgumentException( message );
        }
    }

    private static SpellDefinition ResolveSpell(
        string spellId,
        IReadOnlyCollection<SpellDefinition> spellCatalog )
    {
        return spellCatalog.SingleOrDefault( spell => spell.Id == spellId ) ??
               throw new ArgumentException( $"Spell '{spellId}' is not defined.", nameof( spellCatalog ) );
    }
}

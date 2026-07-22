using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Feats;
using Pathfinder.CharacterManagement.Domain.Rules.Languages;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;
using Pathfinder.CharacterManagement.Domain.Rules.Training;
using Pathfinder.CharacterManagement.Domain.Rules.Equipment;

namespace Pathfinder.CharacterManagement.Application.Completion;

public sealed class CharacterCompletionEvaluator
{
    private readonly IAncestryRepository _ancestryRepository;
    private readonly IBackgroundRepository _backgroundRepository;
    private readonly ICharacterClassRepository _characterClassRepository;
    private readonly IRogueRacketRepository _rogueRacketRepository;
    private readonly IHuntersEdgeRepository _huntersEdgeRepository;
    private readonly IDruidicOrderRepository _druidicOrderRepository;
    private readonly IBardMuseRepository _bardMuseRepository;
    private readonly IWitchPatronRepository _witchPatronRepository;
    private readonly IArcaneSchoolRepository _arcaneSchoolRepository;
    private readonly IArcaneThesisRepository _arcaneThesisRepository;
    private readonly IClericDoctrineRepository _clericDoctrineRepository;
    private readonly IDeityRepository _deityRepository;
    private readonly IClericDomainRepository _clericDomainRepository;
    private readonly ISpellRepository _spellRepository;
    private readonly IFeatRepository _featRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    public CharacterCompletionEvaluator(
        IAncestryRepository ancestryRepository,
        IBackgroundRepository backgroundRepository,
        ICharacterClassRepository characterClassRepository,
        IRogueRacketRepository rogueRacketRepository,
        IHuntersEdgeRepository huntersEdgeRepository,
        IDruidicOrderRepository druidicOrderRepository,
        IBardMuseRepository bardMuseRepository,
        IWitchPatronRepository witchPatronRepository,
        IArcaneSchoolRepository arcaneSchoolRepository,
        IArcaneThesisRepository arcaneThesisRepository,
        IClericDoctrineRepository clericDoctrineRepository,
        IDeityRepository deityRepository,
        IClericDomainRepository clericDomainRepository,
        ISpellRepository spellRepository,
        IFeatRepository featRepository,
        ILanguageRepository languageRepository,
        IEquipmentRepository equipmentRepository )
    {
        _ancestryRepository = ancestryRepository;
        _backgroundRepository = backgroundRepository;
        _characterClassRepository = characterClassRepository;
        _rogueRacketRepository = rogueRacketRepository;
        _huntersEdgeRepository = huntersEdgeRepository;
        _druidicOrderRepository = druidicOrderRepository;
        _bardMuseRepository = bardMuseRepository;
        _witchPatronRepository = witchPatronRepository;
        _arcaneSchoolRepository = arcaneSchoolRepository;
        _arcaneThesisRepository = arcaneThesisRepository;
        _clericDoctrineRepository = clericDoctrineRepository;
        _deityRepository = deityRepository;
        _clericDomainRepository = clericDomainRepository;
        _spellRepository = spellRepository;
        _featRepository = featRepository;
        _languageRepository = languageRepository;
        _equipmentRepository = equipmentRepository;
    }

    public CharacterCompletionDto Evaluate( DraftCharacter character )
    {
        ArgumentNullException.ThrowIfNull( character );

        List<CharacterCompletionIssueDto> issues = [];
        Validate(
            issues,
            CharacterCompletionIssueCode.Identity,
            () => ValidateIdentity( character ) );

        Ancestry? ancestry = Resolve(
            issues,
            CharacterCompletionIssueCode.AncestryPackage,
            () => ResolveAncestry( character ) );
        Background? background = Resolve(
            issues,
            CharacterCompletionIssueCode.BackgroundPackage,
            () => ResolveBackground( character ) );
        CharacterClass? characterClass = Resolve(
            issues,
            CharacterCompletionIssueCode.ClassPackage,
            () => ResolveCharacterClass( character ) );

        Validate(
            issues,
            CharacterCompletionIssueCode.FinalBoosts,
            () => ValidateFinalBoosts( character ) );
        if ( ancestry is not null )
        {
            Validate(
                issues,
                CharacterCompletionIssueCode.Languages,
                () => LanguageSelectionResolver.ResolveSelection(
                    ancestry,
                    character.AbilityScores.Intelligence.Modifier,
                    _languageRepository.GetAll(),
                    character.SelectedAdditionalLanguageIds ) );
        }

        if ( ancestry is not null && background is not null && characterClass is not null )
        {
            ValidateCompletePackages( issues, character, ancestry, background, characterClass );
        }

        return new CharacterCompletionDto
        {
            IsComplete = issues.Count == 0,
            Issues = issues,
        };
    }

    private void ValidateCompletePackages(
        ICollection<CharacterCompletionIssueDto> issues,
        DraftCharacter character,
        Ancestry ancestry,
        Background background,
        CharacterClass characterClass )
    {
        RogueRacket? rogueRacket = null;
        HuntersEdge? huntersEdge = null;
        DruidicOrder? druidicOrder = null;
        BardMuse? bardMuse = null;
        WitchPatron? witchPatron = null;
        ArcaneSchool? arcaneSchool = null;
        ArcaneThesis? arcaneThesis = null;
        ClericDoctrine? clericDoctrine = null;
        Deity? deity = null;
        ClericDomain? clericDomain = null;

        Validate(
            issues,
            CharacterCompletionIssueCode.ClassChoices,
            () => ResolveClassChoices(
                character,
                characterClass,
                out rogueRacket,
                out huntersEdge,
                out druidicOrder,
                out bardMuse,
                out witchPatron,
                out arcaneSchool,
                out arcaneThesis,
                out clericDoctrine,
                out deity,
                out clericDomain ) );
        Validate(
            issues,
            CharacterCompletionIssueCode.SpellLoadout,
            () => ValidateSpellLoadout(
                character,
                characterClass,
                bardMuse,
                druidicOrder,
                witchPatron,
                arcaneSchool,
                deity ) );
        Validate(
            issues,
            CharacterCompletionIssueCode.FeatChoices,
            () => ValidateFeats(
                character,
                background,
                characterClass,
                bardMuse,
                druidicOrder,
                clericDoctrine,
                arcaneSchool,
                arcaneThesis ) );
        Validate(
            issues,
            CharacterCompletionIssueCode.ClassTraining,
            () => ValidateClassTraining( character, characterClass ) );
        Validate(
            issues,
            CharacterCompletionIssueCode.StartingEquipment,
            () => ValidateStartingEquipment(
                character,
                characterClass,
                rogueRacket,
                clericDoctrine,
                deity ) );
    }

    private void ValidateStartingEquipment(
        DraftCharacter character,
        CharacterClass characterClass,
        RogueRacket? rogueRacket,
        ClericDoctrine? clericDoctrine,
        Deity? deity )
    {
        ClassKitDefinition classKit = _equipmentRepository.GetClassKit( characterClass.Id );
        string? deityFavoredWeaponEquipmentId = character.StartingEquipmentItems
            .Select( item => item.EquipmentId )
            .FirstOrDefault( equipmentId => deity?.FavoredWeapons.Any(
                weapon => equipmentId == $"equipment.{weapon.Id[ "weapon.".Length.. ]}" ) == true );
        StartingEquipmentSelection resolved = StartingEquipmentResolver.Resolve(
            classKit,
            _equipmentRepository.GetAll(),
            character.SelectedClassKitOptionIds,
            deity,
            deityFavoredWeaponEquipmentId );

        IReadOnlyList<EffectiveProficiency> proficiencies = ProficiencyResolver.Resolve(
            characterClass.InitialProficiencies
                .Concat( rogueRacket?.ProficiencyGrants ?? [] )
                .Concat( clericDoctrine?.ProficiencyGrants ?? [] )
                .Concat( deity?.ProficiencyGrants ?? [] ) );
        EquipmentLoadoutResult loadout = EquipmentLoadoutResolver.Resolve(
            resolved.Items,
            _equipmentRepository.GetAll(),
            character.StartingEquipmentItems
                .Where( item => item.EquippedQuantity > 0 )
                .Select( item => item.EquipmentId )
                .ToArray(),
            proficiencies,
            character.AbilityScores.Strength.Modifier );

        if ( character.SelectedClassKitId != resolved.ClassKitId ||
             !character.StartingEquipmentItems.SequenceEqual( loadout.Items ) )
        {
            throw new InvalidOperationException( "Starting equipment does not match the selected class kit." );
        }
    }

    private Ancestry ResolveAncestry( DraftCharacter character )
    {
        Ancestry ancestry = _ancestryRepository.GetAncestry( character.AncestryType );
        if ( !character.HasCompleteAncestryPackage )
        {
            throw new InvalidOperationException( "Ancestry heritage and feat choices are required." );
        }

        if ( !ancestry.Heritages.Any( heritage => heritage.Id == character.SelectedHeritageId ) ||
             !ancestry.AncestryFeats.Any( feat => feat.Id == character.SelectedAncestryFeatId ) )
        {
            throw new InvalidOperationException( "Ancestry choices are not available." );
        }

        int requiredFreeBoostCount = ancestry.AbilityBoosts.Count(
            boost => boost is AncestryBoostSlot.FreeBoost );
        if ( character.AppliedFreeBoosts.Count != requiredFreeBoostCount )
        {
            throw new InvalidOperationException( "Ancestry free boosts are incomplete." );
        }

        return ancestry;
    }

    private Background ResolveBackground( DraftCharacter character )
    {
        if ( !character.HasBackgroundBoostPackage || String.IsNullOrWhiteSpace( character.SelectedBackgroundId ) )
        {
            throw new InvalidOperationException( "Background package is incomplete." );
        }

        Background background = _backgroundRepository.GetBackground( character.SelectedBackgroundId );
        if ( String.IsNullOrWhiteSpace( character.SelectedBackgroundSkillFeatId ) )
        {
            throw new InvalidOperationException( "Background skill feat is missing." );
        }

        return background;
    }

    private CharacterClass ResolveCharacterClass( DraftCharacter character )
    {
        if ( !character.HasClassBoostPackage || String.IsNullOrWhiteSpace( character.SelectedClassId ) )
        {
            throw new InvalidOperationException( "Class package is incomplete." );
        }

        CharacterClass characterClass = _characterClassRepository.GetCharacterClass( character.SelectedClassId );
        if ( !character.SelectedClassKeyAbility.HasValue ||
             !characterClass.KeyAbilityOptions.Contains( character.SelectedClassKeyAbility.Value ) )
        {
            throw new InvalidOperationException( "Class key ability is not available." );
        }

        return characterClass;
    }

    private static void ValidateIdentity( DraftCharacter character )
    {
        if ( String.IsNullOrWhiteSpace( character.Name ) ||
             character.Gender == CharacterGender.NotSpecified )
        {
            throw new InvalidOperationException( "Name and gender are required." );
        }
    }

    private static void ValidateFinalBoosts( DraftCharacter character )
    {
        if ( !character.HasFinalFreeBoostPackage )
        {
            throw new InvalidOperationException( "Four final free boosts are required." );
        }
    }

    private void ResolveClassChoices(
        DraftCharacter character,
        CharacterClass characterClass,
        out RogueRacket? rogueRacket,
        out HuntersEdge? huntersEdge,
        out DruidicOrder? druidicOrder,
        out BardMuse? bardMuse,
        out WitchPatron? witchPatron,
        out ArcaneSchool? arcaneSchool,
        out ArcaneThesis? arcaneThesis,
        out ClericDoctrine? clericDoctrine,
        out Deity? deity,
        out ClericDomain? clericDomain )
    {
        rogueRacket = characterClass.Id == "class.rogue"
            ? _rogueRacketRepository.GetRogueRacket( Require( character.SelectedRogueRacketId, "Rogue racket" ) )
            : null;
        huntersEdge = characterClass.Id == "class.ranger"
            ? _huntersEdgeRepository.GetHuntersEdge( Require( character.SelectedHuntersEdgeId, "Hunter's Edge" ) )
            : null;
        druidicOrder = characterClass.Id == "class.druid"
            ? _druidicOrderRepository.GetDruidicOrder( Require( character.SelectedDruidicOrderId, "Druidic Order" ) )
            : null;
        bardMuse = characterClass.Id == "class.bard"
            ? _bardMuseRepository.GetBardMuse( Require( character.SelectedBardMuseId, "Bard Muse" ) )
            : null;
        witchPatron = characterClass.Id == "class.witch"
            ? _witchPatronRepository.GetWitchPatron( Require( character.SelectedWitchPatronId, "Witch Patron" ) )
            : null;
        arcaneSchool = characterClass.Id == "class.wizard"
            ? _arcaneSchoolRepository.GetArcaneSchool( Require( character.SelectedArcaneSchoolId, "Arcane School" ) )
            : null;
        arcaneThesis = characterClass.Id == "class.wizard"
            ? _arcaneThesisRepository.GetArcaneThesis( Require( character.SelectedArcaneThesisId, "Arcane Thesis" ) )
            : null;
        clericDoctrine = characterClass.Id == "class.cleric"
            ? _clericDoctrineRepository.GetClericDoctrine( Require( character.SelectedClericDoctrineId, "Cleric Doctrine" ) )
            : null;
        deity = characterClass.Id == "class.cleric"
            ? _deityRepository.GetDeity( Require( character.SelectedDeityId, "Deity" ) )
            : null;
        bool requiresClericDomain = character.SelectedClericDoctrineId == "cleric_doctrine.cloistered";
        clericDomain = requiresClericDomain
            ? _clericDomainRepository.GetClericDomain( Require( character.SelectedClericDomainId, "Cleric Domain" ) )
            : null;

        if ( characterClass.Id == "class.witch" &&
             String.IsNullOrWhiteSpace( character.SelectedWitchPatronFamiliarSpellId ) )
        {
            throw new InvalidOperationException( "Witch Patron familiar spell is required." );
        }

        if ( characterClass.Id == "class.cleric" &&
             ( !character.SelectedDivineFont.HasValue || !character.SelectedDivineSanctification.HasValue ) )
        {
            throw new InvalidOperationException( "Divine Font and sanctification are required." );
        }
    }

    private void ValidateSpellLoadout(
        DraftCharacter character,
        CharacterClass characterClass,
        BardMuse? bardMuse,
        DruidicOrder? druidicOrder,
        WitchPatron? witchPatron,
        ArcaneSchool? arcaneSchool,
        Deity? deity )
    {
        IReadOnlyCollection<SpellDefinition> spells = _spellRepository.GetAll();
        switch ( characterClass.Id )
        {
            case "class.bard":
                BardSpellLoadoutResolver.Resolve(
                    bardMuse ?? throw new InvalidOperationException( "Bard Muse is required." ),
                    character.BardCantripIds,
                    character.BardSpellIds,
                    spells );
                break;
            case "class.cleric":
                ClericSpellLoadoutResolver.Resolve(
                    deity ?? throw new InvalidOperationException( "Deity is required." ),
                    character.PreparedClericCantripIds,
                    character.PreparedClericSpellIds,
                    spells );
                break;
            case "class.druid":
                _ = druidicOrder ?? throw new InvalidOperationException( "Druidic Order is required." );
                DruidSpellLoadoutResolver.Resolve(
                    character.PreparedDruidCantripIds,
                    character.PreparedDruidSpellIds,
                    spells );
                break;
            case "class.witch":
                string? familiarSpellChoiceId = witchPatron?.FamiliarSpellOptions.Count > 1
                    ? character.SelectedWitchPatronFamiliarSpellId
                    : null;
                WitchSpellLoadoutResolver.Resolve(
                    witchPatron ?? throw new InvalidOperationException( "Witch Patron is required." ),
                    familiarSpellChoiceId,
                    character.WitchFamiliarCantripIds,
                    character.WitchFamiliarSpellIds,
                    character.PreparedWitchCantripIds,
                    character.PreparedWitchSpellIds,
                    Require( character.SelectedWitchFocusHexId, "Witch focus hex" ),
                    spells );
                break;
            case "class.wizard":
                WizardSpellLoadoutResolver.Resolve(
                    arcaneSchool ?? throw new InvalidOperationException( "Arcane School is required." ),
                    character.WizardSpellbookCantripIds,
                    character.WizardSpellbookSpellIds,
                    character.SelectedWizardCurriculumCantripId,
                    character.WizardCurriculumSpellIds,
                    character.PreparedWizardCantripIds,
                    character.PreparedWizardSpellIds,
                    character.SelectedPreparedWizardCurriculumCantripId,
                    character.SelectedPreparedWizardCurriculumSpellId,
                    spells );
                break;
        }
    }

    private void ValidateFeats(
        DraftCharacter character,
        Background background,
        CharacterClass characterClass,
        BardMuse? bardMuse,
        DruidicOrder? druidicOrder,
        ClericDoctrine? clericDoctrine,
        ArcaneSchool? arcaneSchool,
        ArcaneThesis? arcaneThesis )
    {
        IReadOnlyCollection<FeatDefinition> feats = _featRepository.GetAll();
        IReadOnlyCollection<string> grantedFeatIds = GetGrantedClassFeatIds(
            bardMuse,
            druidicOrder,
            clericDoctrine,
            arcaneThesis );
        CharacterFeatResolver.ResolveClassChoices(
            characterClass,
            arcaneSchool,
            arcaneThesis,
            character.SelectedClassFeatChoices,
            feats,
            grantedFeatIds );
        CharacterFeatResolver.Resolve(
            character,
            background,
            characterClass,
            bardMuse,
            druidicOrder,
            clericDoctrine,
            arcaneSchool,
            arcaneThesis,
            feats );
    }

    private static IReadOnlyCollection<string> GetGrantedClassFeatIds(
        BardMuse? bardMuse,
        DruidicOrder? druidicOrder,
        ClericDoctrine? clericDoctrine,
        ArcaneThesis? arcaneThesis )
    {
        List<string> featIds = [];
        BardMuseBenefitDescriptor? museFeat = bardMuse?.Benefits
            .SingleOrDefault( benefit => benefit.Kind == BardMuseBenefitKind.ClassFeat );
        DruidicOrderBenefitDescriptor? orderFeat = druidicOrder?.Benefits
            .SingleOrDefault( benefit => benefit.Kind == DruidicOrderBenefitKind.ClassFeat );
        if ( museFeat is not null )
        {
            featIds.Add( museFeat.Id );
        }
        if ( orderFeat is not null )
        {
            featIds.Add( orderFeat.Id );
        }
        if ( clericDoctrine?.Id == "cleric_doctrine.cloistered" )
        {
            featIds.Add( "feat.domain_initiate" );
        }
        if ( arcaneThesis?.Id == "arcane_thesis.improved_familiar_attunement" )
        {
            featIds.Add( "feat.familiar" );
        }

        return featIds;
    }

    private static void ValidateClassTraining(
        DraftCharacter character,
        CharacterClass characterClass )
    {
        IReadOnlyList<TrainedSkill> trainedSkills = character.TrainedSkills;
        IReadOnlyList<TrainedLore> trainedLore = character.TrainedLore;
        IReadOnlyCollection<string> sourceGrantIds = trainedSkills
            .Select( training => training.SourceGrantId )
            .Concat( trainedLore.Select( training => training.SourceGrantId ) )
            .ToArray();
        if ( characterClass.InitialSkillGrants.Any( grant => !sourceGrantIds.Contains( grant.Id ) ) )
        {
            throw new InvalidOperationException( "Initial class skill grants are incomplete." );
        }

        string additionalSourceGrantId = $"{characterClass.Id}.skill.additional";
        int expectedAdditionalCount = characterClass.AdditionalSkillCountBase +
                                      Math.Max( 0, character.AbilityScores.Intelligence.Modifier );
        if ( sourceGrantIds.Count( sourceId => sourceId == additionalSourceGrantId ) != expectedAdditionalCount )
        {
            throw new InvalidOperationException( "Additional class training is incomplete." );
        }
    }

    private static T? Resolve<T>(
        ICollection<CharacterCompletionIssueDto> issues,
        CharacterCompletionIssueCode code,
        Func<T> resolver ) where T : class
    {
        try
        {
            return resolver();
        }
        catch ( Exception exception ) when ( exception is ArgumentException or InvalidOperationException or Domain.Exceptions.CharacterManagementException )
        {
            AddIssue( issues, code, exception.Message );
            return null;
        }
    }

    private static void Validate(
        ICollection<CharacterCompletionIssueDto> issues,
        CharacterCompletionIssueCode code,
        Action validator )
    {
        try
        {
            validator();
        }
        catch ( Exception exception ) when ( exception is ArgumentException or InvalidOperationException or Domain.Exceptions.CharacterManagementException )
        {
            AddIssue( issues, code, exception.Message );
        }
    }

    private static void AddIssue(
        ICollection<CharacterCompletionIssueDto> issues,
        CharacterCompletionIssueCode code,
        string message )
    {
        if ( issues.All( issue => issue.Code != code ) )
        {
            issues.Add( new CharacterCompletionIssueDto { Code = code, Message = message } );
        }
    }

    private static string Require( string? value, string name )
    {
        if ( String.IsNullOrWhiteSpace( value ) )
        {
            throw new InvalidOperationException( $"{name} is required." );
        }

        return value;
    }
}

using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.CharacterManagement.Domain.Rules.Training;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.CharacterManagement.Domain.Entity;

public class DraftCharacter : Utils.Entities.Base.Entity, IAggregateRoot
{
    private const int ConceptMaxLength = 1000;

    public int AccountId { get; private set; }
    public string Name { get; private set; }
    public string? Concept { get; private set; }
    public int? Age { get; private set; }
    public CharacterGender Gender { get; private set; }
    public AvatarId AvatarId { get; private set; }
    public AncestryType AncestryType { get; private set; }
    public AbilityScores AbilityScores { get; private set; }
    public IReadOnlyList<AbilityType> AppliedFreeBoosts { get; private set; } = [];
    public string? SelectedHeritageId { get; private set; }
    public string? SelectedAncestryFeatId { get; private set; }
    public string? SelectedBackgroundId { get; private set; }
    public AbilityType? SelectedBackgroundRestrictedBoost { get; private set; }
    public AbilityType? SelectedBackgroundFreeBoost { get; private set; }
    public string? SelectedClassId { get; private set; }
    public AbilityType? SelectedClassKeyAbility { get; private set; }
    public string? SelectedRogueRacketId { get; private set; }
    public string? SelectedHuntersEdgeId { get; private set; }
    public string? SelectedDruidicOrderId { get; private set; }
    public string? SelectedBardMuseId { get; private set; }
    public string? SelectedWitchPatronId { get; private set; }
    public string? SelectedWitchPatronFamiliarSpellId { get; private set; }
    public string? SelectedArcaneSchoolId { get; private set; }
    public string? SelectedArcaneThesisId { get; private set; }
    public string? SelectedClericDoctrineId { get; private set; }
    public string? SelectedDeityId { get; private set; }
    public string? SelectedClericDomainId { get; private set; }
    public DivineFont? SelectedDivineFont { get; private set; }
    public DivineSanctification? SelectedDivineSanctification { get; private set; }
    public IReadOnlyList<string> PreparedClericCantripIds { get; private set; } = [];
    public IReadOnlyList<string> PreparedClericSpellIds { get; private set; } = [];
    public IReadOnlyList<string> BardCantripIds { get; private set; } = [];
    public IReadOnlyList<string> BardSpellIds { get; private set; } = [];
    public IReadOnlyList<string> PreparedDruidCantripIds { get; private set; } = [];
    public IReadOnlyList<string> PreparedDruidSpellIds { get; private set; } = [];
    public IReadOnlyList<AbilityType> AppliedFinalFreeBoosts { get; private set; } = [];
    public IReadOnlyList<TrainedSkill> TrainedSkills { get; private set; } = [];
    public IReadOnlyList<TrainedLore> TrainedLore { get; private set; } = [];
    public bool HasCompleteAncestryPackage => !String.IsNullOrWhiteSpace( SelectedHeritageId ) && !String.IsNullOrWhiteSpace( SelectedAncestryFeatId );
    public bool HasBackgroundBoostPackage =>
        !String.IsNullOrWhiteSpace( SelectedBackgroundId ) &&
        SelectedBackgroundRestrictedBoost.HasValue &&
        SelectedBackgroundFreeBoost.HasValue;
    public bool HasClassBoostPackage =>
        !String.IsNullOrWhiteSpace( SelectedClassId ) &&
        SelectedClassKeyAbility.HasValue;
    public bool HasFinalFreeBoostPackage =>
        AppliedFinalFreeBoosts.Count == 4 &&
        AppliedFinalFreeBoosts.Distinct().Count() == 4;

    // Навигационные свойства для EF Core
    public Account Account { get; private set; }

    // Хранится только in-memory, не персистируется в БД
    private Ancestry? _ancestry;

    // Приватный конструктор для EF Core
    private DraftCharacter()
    {
    }

    public static DraftCharacter Create(
        int accountId,
        string name,
        AncestryType ancestryType,
        string? concept = null,
        int? age = null,
        CharacterGender gender = CharacterGender.NotSpecified,
        AvatarId? avatarId = null )
    {
        if ( accountId <= 0 )
        {
            throw new CharacterManagementException( "AccountId must be greater than 0" );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new CharacterManagementException( "Character name cannot be empty" );
        }

        if ( ancestryType == AncestryType.None )
        {
            throw new CharacterManagementException( "AncestryType must be specified" );
        }

        if ( !Enum.IsDefined( gender ) )
        {
            throw new CharacterManagementException( $"Unknown character gender '{gender}'." );
        }

        return new DraftCharacter
        {
            AccountId = accountId,
            Name = name.Trim(),
            Concept = NormalizeConcept( concept ),
            Age = NormalizeAge( age ),
            Gender = gender,
            AvatarId = avatarId ?? AvatarIds.Unknown,
            AncestryType = ancestryType,
            AbilityScores = AbilityScores.CreateDefault()
        };
    }

    public void Rename( string newName )
    {
        ArgumentNullException.ThrowIfNull( newName );

        if ( String.IsNullOrWhiteSpace( newName ) )
        {
            throw new CharacterManagementException( "Character name cannot be empty" );
        }

        if ( newName.Trim() != Name )
        {
            Name = newName.Trim();
            EnsureInvariants();
        }
    }

    public void SetGender( CharacterGender gender )
    {
        if ( Gender != CharacterGender.NotSpecified )
        {
            throw new CharacterManagementException( "Character gender has already been specified." );
        }

        if ( ( gender != CharacterGender.Male ) &&
             ( gender != CharacterGender.Female ) )
        {
            throw new CharacterManagementException( "Character gender must be Male or Female." );
        }

        Gender = gender;
        EnsureInvariants();
    }

    private static string? NormalizeConcept( string? concept )
    {
        if ( String.IsNullOrWhiteSpace( concept ) )
        {
            return null;
        }

        string normalizedConcept = concept.Trim();
        if ( normalizedConcept.Length > ConceptMaxLength )
        {
            throw new CharacterManagementException( $"Character concept cannot exceed {ConceptMaxLength} characters" );
        }

        return normalizedConcept;
    }

    private static int? NormalizeAge( int? age )
    {
        if ( age is null )
        {
            return null;
        }

        if ( age <= 0 )
        {
            throw new CharacterManagementException( "Character age must be greater than zero" );
        }

        // TODO: validate the maximum age against the selected ancestry rules.
        return age;
    }

    public void SetAncestry( Ancestry ancestry )
    {
        ArgumentNullException.ThrowIfNull( ancestry );

        if ( _ancestry is not null )
        {
            RemoveAncestryEffects( _ancestry );
        }

        _ancestry = ancestry;
        AncestryType = ancestry.AncestryType;
        SelectedHeritageId = null;
        SelectedAncestryFeatId = null;
        ApplyAncestryEffects( ancestry );

        EnsureInvariants();
    }

    public void SetAncestryPackage(
        Ancestry? currentAncestry,
        Ancestry nextAncestry,
        string heritageId,
        string ancestryFeatId,
        IAncestryChoiceAvailabilityPolicy availabilityPolicy )
    {
        ArgumentNullException.ThrowIfNull( nextAncestry );
        ArgumentNullException.ThrowIfNull( availabilityPolicy );

        Heritage heritage = GetHeritage( nextAncestry, heritageId );
        AncestryFeat ancestryFeat = GetAncestryFeat( nextAncestry, ancestryFeatId );

        ValidateCurrentAncestry( currentAncestry );
        ValidateAncestryChoiceAvailability( heritage, ancestryFeat, availabilityPolicy );

        Ancestry? appliedAncestry = currentAncestry ?? _ancestry;
        if ( appliedAncestry is not null )
        {
            RemoveAncestryEffects( appliedAncestry );
        }

        _ancestry = nextAncestry;
        AncestryType = nextAncestry.AncestryType;
        SelectedHeritageId = heritage.Id;
        SelectedAncestryFeatId = ancestryFeat.Id;
        ApplyAncestryEffects( nextAncestry );

        EnsureInvariants();
    }

    public void SetFreeBoosts( IReadOnlyList<AbilityType> freeBoosts )
    {
        ArgumentNullException.ThrowIfNull( freeBoosts );

        if ( _ancestry is null )
        {
            throw new CharacterManagementException( "Ancestry must be set before applying free boosts." );
        }

        int freeSlotCount = _ancestry.AbilityBoosts.Count( s => s is AncestryBoostSlot.FreeBoost );

        if ( freeBoosts.Count != freeSlotCount )
        {
            throw new CharacterManagementException(
                $"Expected {freeSlotCount} free boost(s) for {AncestryType}, got {freeBoosts.Count}." );
        }

        if ( freeBoosts.Distinct().Count() != freeBoosts.Count )
        {
            throw new CharacterManagementException( "Free boosts cannot target the same ability twice." );
        }

        HashSet<AbilityType> fixedBoostTypes = _ancestry.AbilityBoosts
            .OfType<AncestryBoostSlot.FixedBoost>()
            .Select( b => b.AbilityType )
            .ToHashSet();

        foreach ( AbilityType boost in freeBoosts )
        {
            if ( fixedBoostTypes.Contains( boost ) )
            {
                throw new CharacterManagementException(
                    $"Cannot apply free boost to {boost}: already boosted by ancestry." );
            }
        }

        // Откат предыдущих free boosts
        foreach ( AbilityType boost in AppliedFreeBoosts )
        {
            AbilityScores.RemoveAbilityBoost( boost );
        }

        // Применяем новые
        foreach ( AbilityType boost in freeBoosts )
        {
            AbilityScores.ApplyAbilityBoost( boost );
        }

        AppliedFreeBoosts = freeBoosts;
        EnsureInvariants();
    }

    public void SetBackgroundPackage(
        Background background,
        AbilityType restrictedBoost,
        AbilityType freeBoost,
        IReadOnlyList<BackgroundTrainingChoice>? trainingChoices = null,
        IReadOnlyCollection<SkillDefinition>? skillCatalog = null )
    {
        ArgumentNullException.ThrowIfNull( background );

        if ( HasClassBoostPackage )
        {
            throw new CharacterManagementException(
                "Background package cannot be replaced after class package has been applied." );
        }

        if ( !background.RestrictedBoostOptions.Contains( restrictedBoost ) )
        {
            throw new CharacterManagementException(
                $"Ability '{restrictedBoost}' is not a restricted boost option for background '{background.Id}'." );
        }

        if ( restrictedBoost == freeBoost )
        {
            throw new CharacterManagementException( "Background boosts must target different abilities." );
        }

        BackgroundTrainingResult training = BackgroundTrainingResolver.Resolve(
            background,
            trainingChoices ?? [],
            skillCatalog ?? [] );

        RemoveBackgroundEffects();

        AbilityScores.ApplyAbilityBoost( restrictedBoost );
        AbilityScores.ApplyAbilityBoost( freeBoost );
        SelectedBackgroundId = background.Id;
        SelectedBackgroundRestrictedBoost = restrictedBoost;
        SelectedBackgroundFreeBoost = freeBoost;
        TrainedSkills = training.Skills.ToArray();
        TrainedLore = training.Lore.ToArray();

        EnsureInvariants();
    }

    public void SetClassPackage(
        CharacterClass characterClass,
        AbilityType keyAbility,
        RogueRacket? rogueRacket = null,
        IReadOnlyList<RogueTrainingChoice>? rogueTrainingChoices = null,
        IReadOnlyCollection<SkillDefinition>? skillCatalog = null,
        ClericDoctrine? clericDoctrine = null,
        Deity? deity = null,
        DivineFont? divineFont = null,
        DivineSanctification? divineSanctification = null,
        string? deitySkillReplacementId = null,
        HuntersEdge? huntersEdge = null,
        DruidicOrder? druidicOrder = null,
        BardMuse? bardMuse = null,
        WitchPatron? witchPatron = null,
        string? witchPatronFamiliarSpellId = null,
        ArcaneSchool? arcaneSchool = null,
        ArcaneThesis? arcaneThesis = null,
        ClericDomain? clericDomain = null,
        ClericSpellLoadout? clericSpellLoadout = null,
        BardSpellLoadout? bardSpellLoadout = null,
        DruidSpellLoadout? druidSpellLoadout = null )
    {
        ArgumentNullException.ThrowIfNull( characterClass );

        if ( !HasBackgroundBoostPackage )
        {
            throw new CharacterManagementException( "Background package must be set before class package." );
        }

        bool isRogue = characterClass.Id == "class.rogue";
        if ( isRogue && rogueRacket is null )
        {
            throw new CharacterManagementException( "Rogue class requires a Rogue's Racket." );
        }

        if ( !isRogue && rogueRacket is not null )
        {
            throw new CharacterManagementException( "Rogue's Racket can only be selected for the Rogue class." );
        }

        if ( !isRogue && rogueTrainingChoices?.Count > 0 )
        {
            throw new CharacterManagementException( "Rogue training choices can only be selected for the Rogue class." );
        }

        bool isRanger = characterClass.Id == "class.ranger";
        if ( isRanger && ( huntersEdge is null ) )
        {
            throw new CharacterManagementException( "Ranger class requires a Hunter's Edge." );
        }

        if ( !isRanger && ( huntersEdge is not null ) )
        {
            throw new CharacterManagementException(
                "Hunter's Edge can only be selected for the Ranger class." );
        }

        bool isDruid = characterClass.Id == "class.druid";
        if ( isDruid && ( druidicOrder is null ) )
        {
            throw new CharacterManagementException( "Druid class requires a Druidic Order." );
        }

        if ( !isDruid && ( druidicOrder is not null ) )
        {
            throw new CharacterManagementException(
                "Druidic Order can only be selected for the Druid class." );
        }

        bool isBard = characterClass.Id == "class.bard";
        if ( isBard && ( bardMuse is null ) )
        {
            throw new CharacterManagementException( "Bard class requires a Muse." );
        }

        if ( !isBard && ( bardMuse is not null ) )
        {
            throw new CharacterManagementException(
                "Bard Muse can only be selected for the Bard class." );
        }

        if ( ( isDruid ) && ( druidSpellLoadout is null ) )
        {
            throw new CharacterManagementException( "Druid class requires a prepared spell loadout." );
        }

        if ( ( !isDruid ) && ( druidSpellLoadout is not null ) )
        {
            throw new CharacterManagementException(
                "Druid spell choices can only be selected for the Druid class." );
        }

        if ( ( isBard ) && ( bardSpellLoadout is null ) )
        {
            throw new CharacterManagementException( "Bard class requires a spell repertoire." );
        }

        if ( ( !isBard ) && ( bardSpellLoadout is not null ) )
        {
            throw new CharacterManagementException(
                "Bard spell choices can only be selected for the Bard class." );
        }

        bool isWitch = characterClass.Id == "class.witch";
        if ( isWitch && ( witchPatron is null ) )
        {
            throw new CharacterManagementException( "Witch class requires a Patron." );
        }

        if ( !isWitch &&
             ( ( witchPatron is not null ) || !String.IsNullOrWhiteSpace( witchPatronFamiliarSpellId ) ) )
        {
            throw new CharacterManagementException(
                "Witch Patron choices can only be selected for the Witch class." );
        }

        bool isWizard = characterClass.Id == "class.wizard";
        if ( isWizard && ( arcaneSchool is null ) )
        {
            throw new CharacterManagementException( "Wizard class requires an Arcane School." );
        }

        if ( !isWizard && ( arcaneSchool is not null ) )
        {
            throw new CharacterManagementException(
                "Arcane School can only be selected for the Wizard class." );
        }

        if ( isWizard && ( arcaneThesis is null ) )
        {
            throw new CharacterManagementException( "Wizard class requires an Arcane Thesis." );
        }

        if ( !isWizard && ( arcaneThesis is not null ) )
        {
            throw new CharacterManagementException(
                "Arcane Thesis can only be selected for the Wizard class." );
        }

        bool isCleric = characterClass.Id == "class.cleric";
        if ( isCleric && clericDoctrine is null )
        {
            throw new CharacterManagementException( "Cleric class requires a Doctrine." );
        }

        if ( !isCleric && clericDoctrine is not null )
        {
            throw new CharacterManagementException( "Cleric Doctrine can only be selected for the Cleric class." );
        }

        if ( isCleric && deity is null )
        {
            throw new CharacterManagementException( "Cleric class requires a Deity." );
        }

        if ( isCleric && deity is not null && !deity.CanGrantClericPowers )
        {
            throw new CharacterManagementException( $"Deity '{deity.Id}' cannot grant Cleric powers." );
        }

        bool isCloisteredCleric = isCleric &&
                                  clericDoctrine?.Id == "cleric_doctrine.cloistered";
        if ( isCloisteredCleric && clericDomain is null )
        {
            throw new CharacterManagementException(
                "Cloistered Cleric requires a primary Deity domain." );
        }

        if ( isCleric && !isCloisteredCleric && clericDomain is not null )
        {
            throw new CharacterManagementException(
                "A first-level Warpriest cannot select a Cleric domain." );
        }

        if ( isCloisteredCleric &&
             deity is not null &&
             clericDomain is not null &&
             !deity.PrimaryDomainIds.Contains( clericDomain.Id, StringComparer.Ordinal ) )
        {
            throw new CharacterManagementException(
                $"Domain '{clericDomain.Id}' is not a primary domain of deity '{deity.Id}'." );
        }

        if ( isCleric && !divineFont.HasValue )
        {
            throw new CharacterManagementException( "Cleric class requires a Divine Font." );
        }

        if ( isCleric && clericSpellLoadout is null )
        {
            throw new CharacterManagementException( "Cleric class requires a prepared spell loadout." );
        }

        if ( isCleric &&
             divineFont.HasValue &&
             deity is not null &&
             !deity.DivineFontOptions.Contains( divineFont.Value ) )
        {
            throw new CharacterManagementException(
                $"Divine Font '{divineFont}' is not allowed for deity '{deity.Id}'." );
        }

        if ( isCleric &&
             deity?.RequiredSanctification is DivineSanctification requiredSanctification &&
             divineSanctification != requiredSanctification )
        {
            throw new CharacterManagementException(
                $"Deity '{deity.Id}' requires {requiredSanctification} sanctification." );
        }

        if ( isCleric &&
             divineSanctification.HasValue &&
             deity is not null &&
             !deity.SanctificationOptions.Contains( divineSanctification.Value ) )
        {
            throw new CharacterManagementException(
                $"Sanctification '{divineSanctification}' is not allowed for deity '{deity.Id}'." );
        }

        if ( !isCleric &&
             ( deity is not null ||
               divineFont.HasValue ||
               divineSanctification.HasValue ||
               clericDomain is not null ||
               !String.IsNullOrWhiteSpace( deitySkillReplacementId ) ||
               clericSpellLoadout is not null ) )
        {
            throw new CharacterManagementException( "Deity class choices can only be selected for the Cleric class." );
        }

        List<AbilityType> allowedKeyAbilities = characterClass.KeyAbilityOptions.ToList();
        if ( rogueRacket?.AlternativeKeyAbility is AbilityType alternativeKeyAbility )
        {
            allowedKeyAbilities.Add( alternativeKeyAbility );
        }

        if ( !allowedKeyAbilities.Contains( keyAbility ) )
        {
            throw new CharacterManagementException(
                $"Ability '{keyAbility}' is not a key ability option for class '{characterClass.Id}'." );
        }

        RogueTrainingResult? rogueTraining = null;
        if ( rogueRacket is not null )
        {
            IReadOnlyList<TrainedSkill> backgroundTraining = TrainedSkills
                .Where( training => training.SourceGrantId.StartsWith( "background.", StringComparison.Ordinal ) )
                .ToArray();
            rogueTraining = RogueTrainingResolver.Resolve(
                rogueRacket,
                rogueTrainingChoices ?? [],
                skillCatalog ?? [],
                backgroundTraining );
        }

        IReadOnlyList<TrainedSkill>? deityTraining = null;
        if ( deity is not null )
        {
            IReadOnlyList<TrainedSkill> backgroundTraining = TrainedSkills
                .Where( training => training.SourceGrantId.StartsWith( "background.", StringComparison.Ordinal ) )
                .ToArray();
            deityTraining = DeityTrainingResolver.Resolve(
                deity,
                deitySkillReplacementId,
                skillCatalog ?? [],
                backgroundTraining );
        }

        WitchPatronBenefitDescriptor? familiarSpell = witchPatron?.ResolveFamiliarSpell(
            witchPatronFamiliarSpellId );

        RemoveClassEffects();

        AbilityScores.ApplyAbilityBoost( keyAbility );
        SelectedClassId = characterClass.Id;
        SelectedClassKeyAbility = keyAbility;
        SelectedRogueRacketId = rogueRacket?.Id;
        SelectedHuntersEdgeId = huntersEdge?.Id;
        SelectedDruidicOrderId = druidicOrder?.Id;
        SelectedBardMuseId = bardMuse?.Id;
        SelectedWitchPatronId = witchPatron?.Id;
        SelectedWitchPatronFamiliarSpellId = familiarSpell?.Id;
        SelectedArcaneSchoolId = arcaneSchool?.Id;
        SelectedArcaneThesisId = arcaneThesis?.Id;
        SelectedClericDoctrineId = clericDoctrine?.Id;
        SelectedDeityId = deity?.Id;
        SelectedClericDomainId = clericDomain?.Id;
        SelectedDivineFont = divineFont;
        SelectedDivineSanctification = divineSanctification;
        PreparedClericCantripIds = clericSpellLoadout?.CantripIds.ToArray() ?? [];
        PreparedClericSpellIds = clericSpellLoadout?.PreparedSpellIds.ToArray() ?? [];
        BardCantripIds = bardSpellLoadout?.CantripIds.ToArray() ?? [];
        BardSpellIds = bardSpellLoadout?.SelectedRankOneSpellIds.ToArray() ?? [];
        PreparedDruidCantripIds = druidSpellLoadout?.CantripIds.ToArray() ?? [];
        PreparedDruidSpellIds = druidSpellLoadout?.PreparedSpellIds.ToArray() ?? [];
        if ( rogueTraining is not null )
        {
            TrainedSkills = rogueTraining.Skills.ToArray();
        }
        else if ( deityTraining is not null )
        {
            TrainedSkills = deityTraining.ToArray();
        }

        EnsureInvariants();
    }

    public void SetFinalFreeBoosts( IReadOnlyList<AbilityType> finalFreeBoosts )
    {
        ArgumentNullException.ThrowIfNull( finalFreeBoosts );

        if ( !HasClassBoostPackage )
        {
            throw new CharacterManagementException( "Class package must be set before final free boosts." );
        }

        if ( finalFreeBoosts.Count != 4 )
        {
            throw new CharacterManagementException(
                $"Expected 4 final free boosts, got {finalFreeBoosts.Count}." );
        }

        if ( finalFreeBoosts.Distinct().Count() != finalFreeBoosts.Count )
        {
            throw new CharacterManagementException( "Final free boosts must target different abilities." );
        }

        foreach ( AbilityType boost in finalFreeBoosts )
        {
            if ( !Enum.IsDefined( boost ) )
            {
                throw new CharacterManagementException( $"Unknown ability type '{boost}'." );
            }

            int previousFinalBoost = AppliedFinalFreeBoosts.Contains( boost ) ? 2 : 0;
            int scoreAfterReplacement = AbilityScores.GetCharacteristic( boost ).Value - previousFinalBoost + 2;
            if ( scoreAfterReplacement > 18 )
            {
                throw new CharacterManagementException(
                    $"Final free boost cannot increase {boost} above 18." );
            }
        }

        RemoveClassTrainingEffects();
        RemoveFinalFreeBoostEffects();

        foreach ( AbilityType boost in finalFreeBoosts )
        {
            AbilityScores.ApplyAbilityBoost( boost );
        }

        AppliedFinalFreeBoosts = finalFreeBoosts.ToArray();
        EnsureInvariants();
    }

    public void SetClassTraining(
        CharacterClass characterClass,
        IReadOnlyList<ClassSkillGrantChoice> grantChoices,
        IReadOnlyList<ClassTrainingTargetChoice> additionalChoices,
        IReadOnlyCollection<SkillDefinition> skillCatalog,
        DruidicOrder? druidicOrder = null,
        WitchPatron? witchPatron = null )
    {
        ArgumentNullException.ThrowIfNull( characterClass );
        ArgumentNullException.ThrowIfNull( grantChoices );
        ArgumentNullException.ThrowIfNull( additionalChoices );
        ArgumentNullException.ThrowIfNull( skillCatalog );

        if ( !HasFinalFreeBoostPackage )
        {
            throw new CharacterManagementException(
                "Final free boost package must be set before class training." );
        }

        if ( characterClass.Id != SelectedClassId )
        {
            throw new CharacterManagementException(
                $"Character class '{characterClass.Id}' does not match selected class '{SelectedClassId}'." );
        }

        if ( !String.Equals(
                 druidicOrder?.Id,
                 SelectedDruidicOrderId,
                 StringComparison.Ordinal ) )
        {
            throw new CharacterManagementException(
                "Druidic Order does not match the selected class package." );
        }

        if ( !String.Equals(
                 witchPatron?.Id,
                 SelectedWitchPatronId,
                 StringComparison.Ordinal ) )
        {
            throw new CharacterManagementException(
                "Witch Patron does not match the selected class package." );
        }

        IReadOnlyList<ClassSkillGrantDescriptor> featureGrants = new[]
            {
                druidicOrder?.SkillGrant,
                witchPatron?.SkillGrant,
            }
            .Where( grant => grant is not null )
            .Cast<ClassSkillGrantDescriptor>()
            .ToArray();

        IReadOnlyList<TrainedSkill> existingSkills = TrainedSkills
            .Where( training => !IsClassTrainingSource( training.SourceGrantId ) )
            .ToArray();
        IReadOnlyList<TrainedLore> existingLore = TrainedLore
            .Where( training => !IsClassTrainingSource( training.SourceGrantId ) )
            .ToArray();
        ClassTrainingResult training = ClassTrainingResolver.Resolve(
            characterClass,
            grantChoices,
            additionalChoices,
            skillCatalog,
            AbilityScores.Intelligence.Modifier,
            existingSkills,
            existingLore,
            featureGrants );

        TrainedSkills = training.Skills.ToArray();
        TrainedLore = training.Lore.ToArray();
        EnsureInvariants();
    }

    public void UpdateAbilityScore( AbilityType abilityType, int value )
    {
        if ( AbilityScores == null )
        {
            throw new CharacterManagementException( "AbilityScores must be initialized before updating" );
        }

        AbilityScores.UpdateCharacteristic( abilityType, value );
        EnsureInvariants();
    }

    private void EnsureInvariants()
    {
        if ( String.IsNullOrWhiteSpace( Name ) )
        {
            throw new CharacterManagementException( "Character name cannot be empty" );
        }

        if ( AncestryType == AncestryType.None )
        {
            throw new CharacterManagementException( "Character must have a valid ancestry" );
        }

        if ( AbilityScores == null )
        {
            throw new CharacterManagementException( "Character must have ability scores" );
        }

        if ( !Enum.IsDefined( Gender ) )
        {
            throw new CharacterManagementException( "Character must have a valid gender state." );
        }

        if ( AvatarId is null )
        {
            throw new CharacterManagementException( "Character must have an avatar identifier." );
        }
    }

    private static Heritage GetHeritage( Ancestry ancestry, string heritageId )
    {
        if ( String.IsNullOrWhiteSpace( heritageId ) )
        {
            throw new CharacterManagementException( "HeritageId must be specified." );
        }

        Heritage? heritage = ancestry.Heritages
            .SingleOrDefault( item => item.Id == heritageId );

        if ( heritage is null )
        {
            throw new CharacterManagementException( $"Heritage '{heritageId}' does not belong to {ancestry.AncestryType}." );
        }

        return heritage;
    }

    private static AncestryFeat GetAncestryFeat( Ancestry ancestry, string ancestryFeatId )
    {
        if ( String.IsNullOrWhiteSpace( ancestryFeatId ) )
        {
            throw new CharacterManagementException( "AncestryFeatId must be specified." );
        }

        AncestryFeat? ancestryFeat = ancestry.AncestryFeats
            .SingleOrDefault( item => item.Id == ancestryFeatId );

        if ( ancestryFeat is null )
        {
            throw new CharacterManagementException( $"Ancestry feat '{ancestryFeatId}' does not belong to {ancestry.AncestryType}." );
        }

        if ( ancestryFeat.Level != 1 )
        {
            throw new CharacterManagementException( $"Ancestry feat '{ancestryFeatId}' must have level 1." );
        }

        return ancestryFeat;
    }

    private void ValidateCurrentAncestry( Ancestry? currentAncestry )
    {
        if ( currentAncestry is null )
        {
            return;
        }

        if ( currentAncestry.AncestryType != AncestryType )
        {
            throw new CharacterManagementException( "Current ancestry does not match the character ancestry." );
        }
    }

    private static void ValidateAncestryChoiceAvailability(
        Heritage heritage,
        AncestryFeat ancestryFeat,
        IAncestryChoiceAvailabilityPolicy availabilityPolicy )
    {
        if ( !availabilityPolicy.IsAvailable( heritage ) )
        {
            throw new CharacterManagementException( $"Heritage '{heritage.Id}' is not available." );
        }

        if ( !availabilityPolicy.IsAvailable( ancestryFeat ) )
        {
            throw new CharacterManagementException( $"Ancestry feat '{ancestryFeat.Id}' is not available." );
        }

        if ( heritage.IncompatibleChoiceIds.Contains( ancestryFeat.Id ) ||
             ancestryFeat.IncompatibleChoiceIds.Contains( heritage.Id ) )
        {
            throw new CharacterManagementException( $"Heritage '{heritage.Id}' and ancestry feat '{ancestryFeat.Id}' are incompatible." );
        }
    }

    private void RemoveAncestryEffects( Ancestry ancestry )
    {
        foreach ( AbilityType boost in AppliedFreeBoosts )
        {
            AbilityScores.RemoveAbilityBoost( boost );
        }

        foreach ( AncestryBoostSlot slot in ancestry.AbilityBoosts )
        {
            if ( slot is AncestryBoostSlot.FixedBoost fixedBoost )
            {
                AbilityScores.RemoveAbilityBoost( fixedBoost.AbilityType );
            }
        }

        foreach ( AbilityType flaw in ancestry.AbilityFlaws )
        {
            AbilityScores.RemoveAbilityFlaw( flaw );
        }

        AppliedFreeBoosts = [];
        SelectedHeritageId = null;
        SelectedAncestryFeatId = null;
    }

    private void ApplyAncestryEffects( Ancestry ancestry )
    {
        foreach ( AncestryBoostSlot slot in ancestry.AbilityBoosts )
        {
            if ( slot is AncestryBoostSlot.FixedBoost fixedBoost )
            {
                AbilityScores.ApplyAbilityBoost( fixedBoost.AbilityType );
            }
        }

        foreach ( AbilityType flaw in ancestry.AbilityFlaws )
        {
            AbilityScores.ApplyAbilityFlaw( flaw );
        }
    }

    private void RemoveBackgroundEffects()
    {
        if ( SelectedBackgroundRestrictedBoost.HasValue )
        {
            AbilityScores.RemoveAbilityBoost( SelectedBackgroundRestrictedBoost.Value );
        }

        if ( SelectedBackgroundFreeBoost.HasValue )
        {
            AbilityScores.RemoveAbilityBoost( SelectedBackgroundFreeBoost.Value );
        }

        SelectedBackgroundId = null;
        SelectedBackgroundRestrictedBoost = null;
        SelectedBackgroundFreeBoost = null;
        TrainedSkills = [];
        TrainedLore = [];
    }

    private void RemoveClassEffects()
    {
        if ( SelectedClassKeyAbility.HasValue )
        {
            AbilityScores.RemoveAbilityBoost( SelectedClassKeyAbility.Value );
        }

        SelectedClassId = null;
        SelectedClassKeyAbility = null;
        SelectedRogueRacketId = null;
        SelectedHuntersEdgeId = null;
        SelectedDruidicOrderId = null;
        SelectedBardMuseId = null;
        SelectedWitchPatronId = null;
        SelectedWitchPatronFamiliarSpellId = null;
        SelectedArcaneSchoolId = null;
        SelectedArcaneThesisId = null;
        SelectedClericDoctrineId = null;
        SelectedDeityId = null;
        SelectedClericDomainId = null;
        SelectedDivineFont = null;
        SelectedDivineSanctification = null;
        PreparedClericCantripIds = [];
        PreparedClericSpellIds = [];
        BardCantripIds = [];
        BardSpellIds = [];
        PreparedDruidCantripIds = [];
        PreparedDruidSpellIds = [];
        RemoveClassTrainingEffects();
        TrainedSkills = TrainedSkills
            .Where( training =>
                !training.SourceGrantId.StartsWith( "rogue_racket.", StringComparison.Ordinal ) &&
                !training.SourceGrantId.StartsWith( "deity.", StringComparison.Ordinal ) )
            .ToArray();
    }

    private void RemoveClassTrainingEffects()
    {
        TrainedSkills = TrainedSkills
            .Where( training => !IsClassTrainingSource( training.SourceGrantId ) )
            .ToArray();
        TrainedLore = TrainedLore
            .Where( training => !IsClassTrainingSource( training.SourceGrantId ) )
            .ToArray();
    }

    private static bool IsClassTrainingSource( string sourceGrantId )
    {
        return ( sourceGrantId.StartsWith( "class.", StringComparison.Ordinal ) ||
                 sourceGrantId.StartsWith( "druidic_order.", StringComparison.Ordinal ) ||
                 sourceGrantId.StartsWith( "witch_patron.", StringComparison.Ordinal ) ) &&
               sourceGrantId.Contains( ".skill.", StringComparison.Ordinal );
    }

    private void RemoveFinalFreeBoostEffects()
    {
        foreach ( AbilityType boost in AppliedFinalFreeBoosts )
        {
            AbilityScores.RemoveAbilityBoost( boost );
        }

        AppliedFinalFreeBoosts = [];
    }
}

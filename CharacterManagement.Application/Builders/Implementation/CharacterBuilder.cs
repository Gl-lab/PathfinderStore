using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.CharacterManagement.Domain.Rules.Training;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace Pathfinder.CharacterManagement.Application.Builders.Implementation;

public class CharacterBuilder : ICharacterBuilder
{
    private DraftCharacter _draftCharacter;
    private readonly IAncestryRepository _ancestryRepository;
    private readonly IAncestryChoiceAvailabilityPolicy _ancestryChoiceAvailabilityPolicy;
    private readonly IBackgroundRepository? _backgroundRepository;
    private readonly ICharacterClassRepository? _characterClassRepository;
    private readonly ISkillRepository? _skillRepository;
    private readonly IRogueRacketRepository? _rogueRacketRepository;
    private readonly IClericDoctrineRepository? _clericDoctrineRepository;
    private readonly IDeityRepository? _deityRepository;
    private readonly IClericDomainRepository? _clericDomainRepository;
    private readonly ISpellRepository? _spellRepository;
    private readonly IHuntersEdgeRepository? _huntersEdgeRepository;
    private readonly IDruidicOrderRepository? _druidicOrderRepository;
    private readonly IBardMuseRepository? _bardMuseRepository;
    private readonly IWitchPatronRepository? _witchPatronRepository;
    private readonly IArcaneSchoolRepository? _arcaneSchoolRepository;
    private readonly IArcaneThesisRepository? _arcaneThesisRepository;

    public CharacterBuilder(
        IAncestryRepository ancestryRepository,
        IAncestryChoiceAvailabilityPolicy? ancestryChoiceAvailabilityPolicy = null,
        IBackgroundRepository? backgroundRepository = null,
        ICharacterClassRepository? characterClassRepository = null,
        ISkillRepository? skillRepository = null,
        IRogueRacketRepository? rogueRacketRepository = null,
        IClericDoctrineRepository? clericDoctrineRepository = null,
        IDeityRepository? deityRepository = null,
        IHuntersEdgeRepository? huntersEdgeRepository = null,
        IDruidicOrderRepository? druidicOrderRepository = null,
        IBardMuseRepository? bardMuseRepository = null,
        IWitchPatronRepository? witchPatronRepository = null,
        IArcaneSchoolRepository? arcaneSchoolRepository = null,
        IArcaneThesisRepository? arcaneThesisRepository = null,
        IClericDomainRepository? clericDomainRepository = null,
        ISpellRepository? spellRepository = null )
    {
        _ancestryRepository = ancestryRepository;
        _ancestryChoiceAvailabilityPolicy = ancestryChoiceAvailabilityPolicy ?? new CommonAncestryChoiceAvailabilityPolicy();
        _backgroundRepository = backgroundRepository;
        _characterClassRepository = characterClassRepository;
        _skillRepository = skillRepository;
        _rogueRacketRepository = rogueRacketRepository;
        _clericDoctrineRepository = clericDoctrineRepository;
        _deityRepository = deityRepository;
        _clericDomainRepository = clericDomainRepository;
        _spellRepository = spellRepository;
        _huntersEdgeRepository = huntersEdgeRepository;
        _druidicOrderRepository = druidicOrderRepository;
        _bardMuseRepository = bardMuseRepository;
        _witchPatronRepository = witchPatronRepository;
        _arcaneSchoolRepository = arcaneSchoolRepository;
        _arcaneThesisRepository = arcaneThesisRepository;
    }

    public void CreateCharacter(
        int accountId,
        string name,
        AncestryType ancestryType,
        string? concept = null,
        int? age = null,
        CharacterGender gender = CharacterGender.NotSpecified,
        AvatarId? avatarId = null ) =>
        _draftCharacter = DraftCharacter.Create( accountId, name, ancestryType, concept, age, gender, avatarId );

    public void SetAncestry( AncestryType ancestryType )
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before setting ancestry." );
        }

        Ancestry ancestry = _ancestryRepository.GetAncestry( ancestryType );
        _draftCharacter.SetAncestry( ancestry );
    }

    public void SetAncestryPackage( string heritageId, string ancestryFeatId )
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before setting ancestry package." );
        }

        Ancestry ancestry = _ancestryRepository.GetAncestry( _draftCharacter.AncestryType );
        _draftCharacter.SetAncestryPackage(
            currentAncestry: null,
            nextAncestry: ancestry,
            heritageId,
            ancestryFeatId,
            _ancestryChoiceAvailabilityPolicy );
    }

    public void SetBackground(
        string backgroundId,
        AbilityType restrictedBoost,
        AbilityType freeBoost,
        IReadOnlyList<BackgroundTrainingChoice>? trainingChoices = null )
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before setting background." );
        }

        if ( _backgroundRepository is null )
        {
            throw new InvalidOperationException( "Background repository is not configured." );
        }

        Background background;
        try
        {
            background = _backgroundRepository.GetBackground( backgroundId );
        }
        catch ( ArgumentException exception )
        {
            throw new CharacterManagementException( exception.Message );
        }

        if ( ( background.Grants.Count > 0 ) && ( _skillRepository is null ) )
        {
            throw new InvalidOperationException( "Skill repository is not configured." );
        }

        _draftCharacter.SetBackgroundPackage(
            background,
            restrictedBoost,
            freeBoost,
            trainingChoices,
            _skillRepository?.GetAll() ?? [] );
    }

    public void SetClass(
        string characterClassId,
        AbilityType keyAbility,
        string? rogueRacketId = null,
        IReadOnlyList<RogueTrainingChoice>? rogueTrainingChoices = null,
        string? clericDoctrineId = null,
        string? deityId = null,
        DivineFont? divineFont = null,
        DivineSanctification? divineSanctification = null,
        string? deitySkillReplacementId = null,
        string? huntersEdgeId = null,
        string? druidicOrderId = null,
        string? bardMuseId = null,
        string? witchPatronId = null,
        string? witchPatronFamiliarSpellId = null,
        string? arcaneSchoolId = null,
        string? arcaneThesisId = null,
        string? clericDomainId = null,
        IReadOnlyList<string>? clericCantripIds = null,
        IReadOnlyList<string>? clericPreparedSpellIds = null,
        IReadOnlyList<string>? bardCantripIds = null,
        IReadOnlyList<string>? bardSpellIds = null,
        IReadOnlyList<string>? druidCantripIds = null,
        IReadOnlyList<string>? druidPreparedSpellIds = null,
        IReadOnlyList<string>? witchFamiliarCantripIds = null,
        IReadOnlyList<string>? witchFamiliarSpellIds = null,
        IReadOnlyList<string>? witchPreparedCantripIds = null,
        IReadOnlyList<string>? witchPreparedSpellIds = null,
        string? witchFocusHexId = null,
        IReadOnlyList<string>? wizardSpellbookCantripIds = null,
        IReadOnlyList<string>? wizardSpellbookSpellIds = null,
        string? wizardCurriculumCantripId = null,
        IReadOnlyList<string>? wizardCurriculumSpellIds = null,
        IReadOnlyList<string>? wizardPreparedCantripIds = null,
        IReadOnlyList<string>? wizardPreparedSpellIds = null,
        string? wizardPreparedCurriculumCantripId = null,
        string? wizardPreparedCurriculumSpellId = null )
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before setting class." );
        }

        if ( _characterClassRepository is null )
        {
            throw new InvalidOperationException( "Character class repository is not configured." );
        }

        CharacterClass characterClass;
        try
        {
            characterClass = _characterClassRepository.GetCharacterClass( characterClassId );
        }
        catch ( ArgumentException exception )
        {
            throw new CharacterManagementException( exception.Message );
        }

        RogueRacket? racket = null;
        if ( !String.IsNullOrWhiteSpace( rogueRacketId ) )
        {
            if ( _rogueRacketRepository is null )
            {
                throw new InvalidOperationException( "Rogue racket repository is not configured." );
            }

            try
            {
                racket = _rogueRacketRepository.GetRogueRacket( rogueRacketId );
            }
            catch ( ArgumentException exception )
            {
                throw new CharacterManagementException( exception.Message );
            }
        }

        ClericDoctrine? doctrine = null;
        if ( !String.IsNullOrWhiteSpace( clericDoctrineId ) )
        {
            if ( _clericDoctrineRepository is null )
            {
                throw new InvalidOperationException( "Cleric doctrine repository is not configured." );
            }

            try
            {
                doctrine = _clericDoctrineRepository.GetClericDoctrine( clericDoctrineId );
            }
            catch ( ArgumentException exception )
            {
                throw new CharacterManagementException( exception.Message );
            }
        }

        Deity? deity = null;
        if ( !String.IsNullOrWhiteSpace( deityId ) )
        {
            if ( _deityRepository is null )
            {
                throw new InvalidOperationException( "Deity repository is not configured." );
            }

            try
            {
                deity = _deityRepository.GetDeity( deityId );
            }
            catch ( ArgumentException exception )
            {
                throw new CharacterManagementException( exception.Message );
            }
        }

        ClericDomain? clericDomain = null;
        if ( !String.IsNullOrWhiteSpace( clericDomainId ) )
        {
            if ( _clericDomainRepository is null )
            {
                throw new InvalidOperationException( "Cleric domain repository is not configured." );
            }

            try
            {
                clericDomain = _clericDomainRepository.GetClericDomain( clericDomainId );
            }
            catch ( ArgumentException exception )
            {
                throw new CharacterManagementException( exception.Message );
            }
        }

        HuntersEdge? huntersEdge = null;
        if ( !String.IsNullOrWhiteSpace( huntersEdgeId ) )
        {
            if ( _huntersEdgeRepository is null )
            {
                throw new InvalidOperationException( "Hunter's Edge repository is not configured." );
            }

            try
            {
                huntersEdge = _huntersEdgeRepository.GetHuntersEdge( huntersEdgeId );
            }
            catch ( ArgumentException exception )
            {
                throw new CharacterManagementException( exception.Message );
            }
        }

        DruidicOrder? druidicOrder = null;
        if ( !String.IsNullOrWhiteSpace( druidicOrderId ) )
        {
            if ( _druidicOrderRepository is null )
            {
                throw new InvalidOperationException( "Druidic Order repository is not configured." );
            }

            try
            {
                druidicOrder = _druidicOrderRepository.GetDruidicOrder( druidicOrderId );
            }
            catch ( ArgumentException exception )
            {
                throw new CharacterManagementException( exception.Message );
            }
        }

        BardMuse? bardMuse = null;
        if ( !String.IsNullOrWhiteSpace( bardMuseId ) )
        {
            if ( _bardMuseRepository is null )
            {
                throw new InvalidOperationException( "Bard Muse repository is not configured." );
            }

            try
            {
                bardMuse = _bardMuseRepository.GetBardMuse( bardMuseId );
            }
            catch ( ArgumentException exception )
            {
                throw new CharacterManagementException( exception.Message );
            }
        }

        WitchPatron? witchPatron = null;
        if ( !String.IsNullOrWhiteSpace( witchPatronId ) )
        {
            if ( _witchPatronRepository is null )
            {
                throw new InvalidOperationException( "Witch Patron repository is not configured." );
            }

            try
            {
                witchPatron = _witchPatronRepository.GetWitchPatron( witchPatronId );
            }
            catch ( ArgumentException exception )
            {
                throw new CharacterManagementException( exception.Message );
            }
        }

        ArcaneSchool? arcaneSchool = null;
        if ( !String.IsNullOrWhiteSpace( arcaneSchoolId ) )
        {
            if ( _arcaneSchoolRepository is null )
            {
                throw new InvalidOperationException( "Arcane School repository is not configured." );
            }

            try
            {
                arcaneSchool = _arcaneSchoolRepository.GetArcaneSchool( arcaneSchoolId );
            }
            catch ( ArgumentException exception )
            {
                throw new CharacterManagementException( exception.Message );
            }
        }

        ArcaneThesis? arcaneThesis = null;
        if ( !String.IsNullOrWhiteSpace( arcaneThesisId ) )
        {
            if ( _arcaneThesisRepository is null )
            {
                throw new InvalidOperationException( "Arcane Thesis repository is not configured." );
            }

            try
            {
                arcaneThesis = _arcaneThesisRepository.GetArcaneThesis( arcaneThesisId );
            }
            catch ( ArgumentException exception )
            {
                throw new CharacterManagementException( exception.Message );
            }
        }

        if ( ( ( characterClass.Id == "class.rogue" ) || ( deity is not null ) ) &&
             ( _skillRepository is null ) )
        {
            throw new InvalidOperationException( "Skill repository is not configured." );
        }

        ClericSpellLoadout? clericSpellLoadout = null;
        bool isCleric = characterClass.Id == "class.cleric";
        if ( !isCleric &&
             ( ( clericCantripIds?.Count > 0 ) || ( clericPreparedSpellIds?.Count > 0 ) ) )
        {
            throw new CharacterManagementException(
                "Cleric spell choices can only be selected for the Cleric class." );
        }

        if ( isCleric && ( deity is not null ) )
        {
            if ( _spellRepository is null )
            {
                throw new InvalidOperationException( "Spell repository is not configured." );
            }

            clericSpellLoadout = ClericSpellLoadoutResolver.Resolve(
                deity,
                clericCantripIds ?? [],
                clericPreparedSpellIds ?? [],
                _spellRepository.GetAll() );
        }

        BardSpellLoadout? bardSpellLoadout = null;
        bool isBard = characterClass.Id == "class.bard";
        if ( ( !isBard ) &&
             ( ( bardCantripIds?.Count > 0 ) || ( bardSpellIds?.Count > 0 ) ) )
        {
            throw new CharacterManagementException(
                "Bard spell choices can only be selected for the Bard class." );
        }

        if ( ( isBard ) && ( bardMuse is not null ) )
        {
            if ( _spellRepository is null )
            {
                throw new InvalidOperationException( "Spell repository is not configured." );
            }

            bardSpellLoadout = BardSpellLoadoutResolver.Resolve(
                bardMuse,
                bardCantripIds ?? [],
                bardSpellIds ?? [],
                _spellRepository.GetAll() );
        }

        DruidSpellLoadout? druidSpellLoadout = null;
        bool isDruid = characterClass.Id == "class.druid";
        if ( ( !isDruid ) &&
             ( ( druidCantripIds?.Count > 0 ) || ( druidPreparedSpellIds?.Count > 0 ) ) )
        {
            throw new CharacterManagementException(
                "Druid spell choices can only be selected for the Druid class." );
        }

        if ( isDruid )
        {
            if ( _spellRepository is null )
            {
                throw new InvalidOperationException( "Spell repository is not configured." );
            }

            druidSpellLoadout = DruidSpellLoadoutResolver.Resolve(
                druidCantripIds ?? [],
                druidPreparedSpellIds ?? [],
                _spellRepository.GetAll() );
        }

        WitchSpellLoadout? witchSpellLoadout = null;
        bool isWitch = characterClass.Id == "class.witch";
        if ( isWitch )
        {
            if ( ( witchPatron is null ) || ( _spellRepository is null ) )
            {
                throw new InvalidOperationException( "Witch Patron and spell repositories are required." );
            }

            witchSpellLoadout = WitchSpellLoadoutResolver.Resolve(
                witchPatron,
                witchPatronFamiliarSpellId,
                witchFamiliarCantripIds ?? [],
                witchFamiliarSpellIds ?? [],
                witchPreparedCantripIds ?? [],
                witchPreparedSpellIds ?? [],
                witchFocusHexId ?? String.Empty,
                _spellRepository.GetAll() );
        }
        else if ( ( witchFamiliarCantripIds?.Count > 0 ) ||
                  ( witchFamiliarSpellIds?.Count > 0 ) ||
                  ( witchPreparedCantripIds?.Count > 0 ) ||
                  ( witchPreparedSpellIds?.Count > 0 ) ||
                  !String.IsNullOrWhiteSpace( witchFocusHexId ) )
        {
            throw new CharacterManagementException(
                "Witch spell choices can only be selected for the Witch class." );
        }

        WizardSpellLoadout? wizardSpellLoadout = null;
        bool isWizard = characterClass.Id == "class.wizard";
        bool hasWizardSpellChoices =
            ( wizardSpellbookCantripIds?.Count > 0 ) ||
            ( wizardSpellbookSpellIds?.Count > 0 ) ||
            !String.IsNullOrWhiteSpace( wizardCurriculumCantripId ) ||
            ( wizardCurriculumSpellIds?.Count > 0 ) ||
            ( wizardPreparedCantripIds?.Count > 0 ) ||
            ( wizardPreparedSpellIds?.Count > 0 ) ||
            !String.IsNullOrWhiteSpace( wizardPreparedCurriculumCantripId ) ||
            !String.IsNullOrWhiteSpace( wizardPreparedCurriculumSpellId );
        if ( isWizard && hasWizardSpellChoices )
        {
            if ( ( arcaneSchool is null ) || ( _spellRepository is null ) )
            {
                throw new InvalidOperationException( "Wizard Arcane School and spell repositories are required." );
            }

            wizardSpellLoadout = WizardSpellLoadoutResolver.Resolve(
                arcaneSchool,
                wizardSpellbookCantripIds ?? [],
                wizardSpellbookSpellIds ?? [],
                wizardCurriculumCantripId,
                wizardCurriculumSpellIds ?? [],
                wizardPreparedCantripIds ?? [],
                wizardPreparedSpellIds ?? [],
                wizardPreparedCurriculumCantripId,
                wizardPreparedCurriculumSpellId,
                _spellRepository.GetAll() );
        }
        else if ( !isWizard && hasWizardSpellChoices )
        {
            throw new CharacterManagementException(
                "Wizard spell choices can only be selected for the Wizard class." );
        }

        _draftCharacter.SetClassPackage(
            characterClass,
            keyAbility,
            racket,
            rogueTrainingChoices,
            _skillRepository?.GetAll() ?? [],
            doctrine,
            deity,
            divineFont,
            divineSanctification,
            deitySkillReplacementId,
            huntersEdge,
            druidicOrder,
            bardMuse,
            witchPatron,
            witchPatronFamiliarSpellId,
            arcaneSchool,
            arcaneThesis,
            clericDomain,
            clericSpellLoadout,
            bardSpellLoadout,
            druidSpellLoadout,
            witchSpellLoadout,
            wizardSpellLoadout );
    }

    public void SetFinalFreeBoosts( IReadOnlyList<AbilityType> finalFreeBoosts )
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before setting final free boosts." );
        }

        _draftCharacter.SetFinalFreeBoosts( finalFreeBoosts );
    }

    public void SetClassTraining(
        string characterClassId,
        IReadOnlyList<ClassSkillGrantChoice> grantChoices,
        IReadOnlyList<ClassTrainingTargetChoice> additionalChoices )
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before setting class training." );
        }

        if ( _characterClassRepository is null )
        {
            throw new InvalidOperationException( "Character class repository is not configured." );
        }

        if ( _skillRepository is null )
        {
            throw new InvalidOperationException( "Skill repository is not configured." );
        }

        CharacterClass characterClass;
        try
        {
            characterClass = _characterClassRepository.GetCharacterClass( characterClassId );
        }
        catch ( ArgumentException exception )
        {
            throw new CharacterManagementException( exception.Message );
        }

        _draftCharacter.SetClassTraining(
            characterClass,
            grantChoices,
            additionalChoices,
            _skillRepository.GetAll(),
            ResolveSelectedDruidicOrder(),
            ResolveSelectedWitchPatron() );
    }

    private DruidicOrder? ResolveSelectedDruidicOrder()
    {
        if ( _draftCharacter?.SelectedDruidicOrderId is null )
        {
            return null;
        }

        if ( _druidicOrderRepository is null )
        {
            throw new InvalidOperationException( "Druidic Order repository is not configured." );
        }

        return _druidicOrderRepository.GetDruidicOrder( _draftCharacter.SelectedDruidicOrderId );
    }

    private WitchPatron? ResolveSelectedWitchPatron()
    {
        if ( _draftCharacter?.SelectedWitchPatronId is null )
        {
            return null;
        }

        if ( _witchPatronRepository is null )
        {
            throw new InvalidOperationException( "Witch Patron repository is not configured." );
        }

        return _witchPatronRepository.GetWitchPatron( _draftCharacter.SelectedWitchPatronId );
    }

    public void IncreaseAbilityScores( IEnumerable<AbilityType> increasedAbilityTypes )
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before increasing ability scores." );
        }

        foreach ( AbilityType abilityType in increasedAbilityTypes )
        {
            Characteristic current = _draftCharacter.AbilityScores.GetCharacteristic( abilityType );
            int newValue = current.Value + 2;
            _draftCharacter.UpdateAbilityScore( abilityType, newValue );
        }
    }

    public void ApplyFreeBoosts( IEnumerable<AbilityType> freeBoosts )
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before applying free boosts." );
        }

        _draftCharacter.SetFreeBoosts( freeBoosts.ToList() );
    }

    public void SetAbilityScores() => throw new NotImplementedException();

    public void SetInventory() => throw new NotImplementedException();

    public void SetAlignment() => throw new NotImplementedException();

    public void SetDeity() => throw new NotImplementedException();

    public void SetAge() => throw new NotImplementedException();

    public void SetGender( CharacterGender gender )
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before setting gender." );
        }

        _draftCharacter.SetGender( gender );
    }

    public void SetName( string name )
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before setting name." );
        }

        _draftCharacter.Rename( name );
    }

    public DraftCharacter Build()
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before building." );
        }

        return _draftCharacter;
    }
}

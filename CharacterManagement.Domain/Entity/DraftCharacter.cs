using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.CharacterManagement.Domain.Entity;

public class DraftCharacter : Utils.Entities.Base.Entity, IAggregateRoot
{
    private const int ConceptMaxLength = 1000;

    public int AccountId { get; private set; }
    public string Name { get; private set; }
    public string? Concept { get; private set; }
    public int? Age { get; private set; }
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
        int? age = null )
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

        return new DraftCharacter
        {
            AccountId = accountId,
            Name = name.Trim(),
            Concept = NormalizeConcept( concept ),
            Age = NormalizeAge( age ),
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
        IReadOnlyCollection<SkillDefinition>? skillCatalog = null )
    {
        ArgumentNullException.ThrowIfNull( characterClass );

        if ( !HasBackgroundBoostPackage )
        {
            throw new CharacterManagementException( "Background package must be set before class package." );
        }

        bool isRogue = characterClass.Id == "class.rogue";
        if ( isRogue && ( rogueRacket is null ) )
        {
            throw new CharacterManagementException( "Rogue class requires a Rogue's Racket." );
        }

        if ( !isRogue && ( rogueRacket is not null ) )
        {
            throw new CharacterManagementException( "Rogue's Racket can only be selected for the Rogue class." );
        }

        if ( !isRogue && ( rogueTrainingChoices?.Count > 0 ) )
        {
            throw new CharacterManagementException( "Rogue training choices can only be selected for the Rogue class." );
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

        RemoveClassEffects();

        AbilityScores.ApplyAbilityBoost( keyAbility );
        SelectedClassId = characterClass.Id;
        SelectedClassKeyAbility = keyAbility;
        SelectedRogueRacketId = rogueRacket?.Id;
        if ( rogueTraining is not null )
        {
            TrainedSkills = rogueTraining.Skills.ToArray();
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

        RemoveFinalFreeBoostEffects();

        foreach ( AbilityType boost in finalFreeBoosts )
        {
            AbilityScores.ApplyAbilityBoost( boost );
        }

        AppliedFinalFreeBoosts = finalFreeBoosts.ToArray();
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
        TrainedSkills = TrainedSkills
            .Where( training =>
                !training.SourceGrantId.StartsWith( "class.", StringComparison.Ordinal ) &&
                !training.SourceGrantId.StartsWith( "rogue_racket.", StringComparison.Ordinal ) )
            .ToArray();
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

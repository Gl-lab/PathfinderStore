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

    public void ChangeAncestryType( AncestryType ancestryType )
    {
        if ( ancestryType == AncestryType.None )
        {
            throw new CharacterManagementException( "AncestryType must be specified" );
        }

        AncestryType = ancestryType;
        EnsureInvariants();
    }

    public void SetAncestry( Ancestry ancestry )
    {
        ArgumentNullException.ThrowIfNull( ancestry );

        // Откат предыдущей расы если была установлена
        if ( _ancestry is not null )
        {
            // Сначала откатываем free boosts
            foreach ( AbilityType boost in AppliedFreeBoosts )
            {
                AbilityScores.RemoveAbilityBoost( boost );
            }

            // Откатываем fixed boosts
            foreach ( AncestryBoostSlot slot in _ancestry.AbilityBoosts )
            {
                if ( slot is AncestryBoostSlot.FixedBoost fixedBoost )
                {
                    AbilityScores.RemoveAbilityBoost( fixedBoost.AbilityType );
                }
            }

            // Отменяем флои
            foreach ( AbilityType flaw in _ancestry.AbilityFlaws )
            {
                AbilityScores.RemoveAbilityFlaw( flaw );
            }

            AppliedFreeBoosts = [];
        }

        _ancestry = ancestry;
        AncestryType = ancestry.AncestryType;

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
}

using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.CharacterManagement.Domain.Entity;

public class DraftCharacter : Utils.Entities.Base.Entity, IAggregateRoot
{
    public int UserId { get; private set; }
    public string Name { get; private set; }
    public int RaceId { get; private set; }
    public AncestryType? AncestryType { get; private set; }
    public AbilityScores AbilityScores { get; private set; }

    // Навигационные свойства для EF Core (опционально, только для загрузки связанных данных)
    public Race Race { get; private set; }

    // Приватный конструктор для EF Core
    private DraftCharacter()
    {
    }

    public static DraftCharacter Create(
        int userId,
        string name,
        int raceId )
    {
        if ( userId <= 0 )
        {
            throw new CharacterManagementException( "UserId must be greater than 0" );
        }

        if ( String.IsNullOrWhiteSpace( name ) )
        {
            throw new CharacterManagementException( "Character name cannot be empty" );
        }

        if ( raceId <= 0 )
        {
            throw new CharacterManagementException( "RaceId must be greater than 0" );
        }

        return new DraftCharacter
        {
            UserId = userId,
            Name = name.Trim(),
            RaceId = raceId,
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

    public void ChangeRace( int raceId )
    {
        if ( raceId <= 0 )
        {
            throw new CharacterManagementException( "RaceId must be greater than 0" );
        }

        RaceId = raceId;

        EnsureInvariants();
    }

    public void SetAncestry( Ancestry ancestry )
    {
        ArgumentNullException.ThrowIfNull( ancestry );

        AncestryType = ancestry.AncestryType;

        // Применяем бонусы
        foreach ( AbilityType abilityBoost in ancestry.AbilityBoosts )
        {
            AbilityScores.ApplyAbilityBoost( abilityBoost );
        }

        // Применяем штрафы
        foreach ( AbilityType abilityFlaw in ancestry.AbilityFlaws )
        {
            AbilityScores.ApplyAbilityFlaw( abilityFlaw );
        }

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

        if ( RaceId <= 0 )
        {
            throw new CharacterManagementException( "Character must have a valid race" );
        }

        if ( AbilityScores == null )
        {
            throw new CharacterManagementException( "Character must have ability scores" );
        }
    }
}
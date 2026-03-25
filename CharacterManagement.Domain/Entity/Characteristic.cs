using Pathfinder.CharacterManagement.Domain.Exceptions;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.CharacterManagement.Domain.Entity;

// Characteristic — числовое значение характеристики.
// Equality по Value: два Characteristic(10) равны, идентификация через позицию в AbilityScores (Strength, Dexterity и т.д.)
public class Characteristic : ValueObject
{
    public int Value { get; }
    public int Modifier => ( Value - 10 ) / 2;

    // Приватный конструктор для EF Core
    private Characteristic()
    {
    }

    public Characteristic( int value )
    {
        if ( value < 1 || value > 30 )
        {
            throw new CharacterManagementException( $"Ability score must be between 1 and 30, got {value}" );
        }

        Value = value;
    }

    public Characteristic Increase( int amount )
    {
        int newValue = Value + amount;
        if ( newValue > 30 )
        {
            throw new CharacterManagementException( $"Ability score cannot exceed 30. Current: {Value}, Attempted: {newValue}" );
        }

        return new Characteristic( newValue );
    }

    public Characteristic Decrease( int amount )
    {
        int newValue = Value - amount;
        if ( newValue < 1 )
        {
            throw new CharacterManagementException( $"Ability score cannot be below 1. Current: {Value}, Attempted: {newValue}" );
        }

        return new Characteristic( newValue );
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
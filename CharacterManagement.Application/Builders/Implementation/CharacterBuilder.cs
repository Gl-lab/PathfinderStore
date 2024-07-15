using System.Collections.Generic;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Repositories;

namespace Pathfinder.Application.Builders.Implementation;

public class CharacterBuilder : ICharacterBuilder
{
    private Character _character;
    private readonly IAncestryRepository _ancestryRepository;

    public CharacterBuilder( IAncestryRepository ancestryRepository )
    {
        _ancestryRepository = ancestryRepository;
    }

    public void CreateCharacter()
    {
        _character = new Character
        {
            AbilityScores = InitializationAbilityScores()
        };
    }

    public void SetAncestry( AncestryType ancestryType )
    {
        _character.AncestryType = ancestryType;
        Ancestry ancestry = _ancestryRepository.GetAncestry( ancestryType );
        foreach ( AbilityType ancestryAbilityBoost in ancestry.AbilityBoosts )
        {
            _character.AbilityScores.GetCharacteristic( ancestryAbilityBoost ).Value += 2;
        }
        foreach ( AbilityType ancestryAbilityFlaw in ancestry.AbilityFlaws )
        {
            _character.AbilityScores.GetCharacteristic( ancestryAbilityFlaw ).Value += 2;
        }
    }

    private static AbilityScores InitializationAbilityScores()
    {
        AbilityScores abilityScores = new()
        {
            Charisma = new Characteristic()
            {
                Value = 10,
                AbilityType = AbilityType.Charisma
            },
            Constitution = new Characteristic()
            {
                Value = 10,
                AbilityType = AbilityType.Constitution
            },
            Wisdom = new Characteristic()
            {
                Value = 10,
                AbilityType = AbilityType.Wisdom
            },
            Dexterity = new Characteristic()
            {
                Value = 10,
                AbilityType = AbilityType.Dexterity
            },
            Intelligence = new Characteristic()
            {
                Value = 10,
                AbilityType = AbilityType.Intelligence
            },
            Strength = new Characteristic()
            {
                Value = 10,
                AbilityType = AbilityType.Strength
            }
        };
        return abilityScores;
    }

    public void SetBackground()
    {
        throw new System.NotImplementedException();
    }

    public void SetClass()
    {
        throw new System.NotImplementedException();
    }

    public void IncreaseAbilityScores( IEnumerable<AbilityType> increasedAbilityTypes )
    {
        foreach ( AbilityType abilityType  in increasedAbilityTypes )
        {
            Characteristic characteristic = _character.AbilityScores.GetCharacteristic( abilityType );
            characteristic.Value += 2;
        }
        
    }

    public void SetAbilityScores()
    {
        throw new System.NotImplementedException();
    }

    public void SetInventory()
    {
        throw new System.NotImplementedException();
    }

    public void SetAlignment()
    {
        throw new System.NotImplementedException();
    }

    public void SetDeity()
    {
        throw new System.NotImplementedException();
    }

    public void SetAge()
    {
        throw new System.NotImplementedException();
    }

    public void SetGender()
    {
        throw new System.NotImplementedException();
    }

    public void SetName( string name )
    {
        _character.Name = name;
    }
}
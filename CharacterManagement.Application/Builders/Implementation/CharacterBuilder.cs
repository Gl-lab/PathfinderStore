using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Builders.Implementation;

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
        if ( _character is null )
        {
            throw new InvalidOperationException( "Character must be created before setting ancestry." );
        }
        Ancestry ancestry = _ancestryRepository.GetAncestry( ancestryType );
        _character.SetAncestry( ancestry );
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
        _character.Rename( name );
    }
}
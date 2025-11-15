using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Builders.Implementation;

public class CharacterBuilder : ICharacterBuilder
{
    private DraftCharacter _draftCharacter;
    private readonly IAncestryRepository _ancestryRepository;

    public CharacterBuilder( IAncestryRepository ancestryRepository )
    {
        _ancestryRepository = ancestryRepository;
    }

    public void CreateCharacter()
    {
        _draftCharacter = new DraftCharacter();
    }

    public void SetAncestry( AncestryType ancestryType )
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before setting ancestry." );
        }

        Ancestry ancestry = _ancestryRepository.GetAncestry( ancestryType );
        _draftCharacter.SetAncestry( ancestry );
    }

    public void SetBackground()
    {
        throw new NotImplementedException();
    }

    public void SetClass()
    {
        throw new NotImplementedException();
    }

    public void IncreaseAbilityScores( IEnumerable<AbilityType> increasedAbilityTypes )
    {
        foreach ( AbilityType abilityType in increasedAbilityTypes )
        {
            Characteristic characteristic = _draftCharacter.AbilityScores.GetCharacteristic( abilityType );
            characteristic.Value += 2;
        }
    }

    public void SetAbilityScores()
    {
        throw new NotImplementedException();
    }

    public void SetInventory()
    {
        throw new NotImplementedException();
    }

    public void SetAlignment()
    {
        throw new NotImplementedException();
    }

    public void SetDeity()
    {
        throw new NotImplementedException();
    }

    public void SetAge()
    {
        throw new NotImplementedException();
    }

    public void SetGender()
    {
        throw new NotImplementedException();
    }

    public void SetName( string name )
    {
        _draftCharacter.Rename( name );
    }
}
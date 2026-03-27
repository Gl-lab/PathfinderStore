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

    public void CreateCharacter(
        int accountId,
        string name,
        int raceId ) =>
        _draftCharacter = DraftCharacter.Create( accountId, name, raceId );

    public void SetAncestry( AncestryType ancestryType )
    {
        if ( _draftCharacter is null )
        {
            throw new InvalidOperationException( "Character must be created before setting ancestry." );
        }

        Ancestry ancestry = _ancestryRepository.GetAncestry( ancestryType );
        _draftCharacter.SetAncestry( ancestry );
    }

    public void SetBackground() => throw new NotImplementedException();

    public void SetClass() => throw new NotImplementedException();

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

    public void SetGender() => throw new NotImplementedException();

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
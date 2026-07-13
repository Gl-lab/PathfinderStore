using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Exceptions;

namespace Pathfinder.CharacterManagement.Application.Builders.Implementation;

public class CharacterBuilder : ICharacterBuilder
{
    private DraftCharacter _draftCharacter;
    private readonly IAncestryRepository _ancestryRepository;
    private readonly IAncestryChoiceAvailabilityPolicy _ancestryChoiceAvailabilityPolicy;
    private readonly IBackgroundRepository? _backgroundRepository;

    public CharacterBuilder(
        IAncestryRepository ancestryRepository,
        IAncestryChoiceAvailabilityPolicy? ancestryChoiceAvailabilityPolicy = null,
        IBackgroundRepository? backgroundRepository = null )
    {
        _ancestryRepository = ancestryRepository;
        _ancestryChoiceAvailabilityPolicy = ancestryChoiceAvailabilityPolicy ?? new CommonAncestryChoiceAvailabilityPolicy();
        _backgroundRepository = backgroundRepository;
    }

    public void CreateCharacter(
        int accountId,
        string name,
        AncestryType ancestryType,
        string? concept = null,
        int? age = null ) =>
        _draftCharacter = DraftCharacter.Create( accountId, name, ancestryType, concept, age );

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
        AbilityType freeBoost )
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

        _draftCharacter.SetBackgroundPackage( background, restrictedBoost, freeBoost );
    }

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

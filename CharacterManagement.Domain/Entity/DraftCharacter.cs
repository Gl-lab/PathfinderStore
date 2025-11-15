using Pathfinder.Contracts;

namespace Pathfinder.CharacterManagement.Domain.Entity;

public class DraftCharacter : Utils.Entities.Base.Entity
{
    public Ancestry Ancestry { get; private set; }

    public string Name { get; private set; }

    // public int RaceId { get; set; }
    public Race Race { get; private set; }
    public AbilityScores AbilityScores { get; private set; }
    public IUser User { get; set; }

    public void Rename( string newName )
    {
        ArgumentNullException.ThrowIfNull( newName );

        if ( !String.IsNullOrWhiteSpace( newName ) && newName != Name )
        {
            Name = newName;
        }
    }

    public void ChangeRace( Race newRace )
    {
        ArgumentNullException.ThrowIfNull( newRace );
        Race = newRace;
    }

    public void SetAncestry( Ancestry ancestry )
    {
        ArgumentNullException.ThrowIfNull( ancestry );
        Ancestry = ancestry;
        if ( AbilityScores is null )
        {
            AbilityScores = AbilityScores.InitializationAbilityScores();
        }

        foreach ( AbilityType ancestryAbilityBoost in ancestry.AbilityBoosts )
        {
            AbilityScores.GetCharacteristic( ancestryAbilityBoost )
               .Value += 2;
        }

        foreach ( AbilityType ancestryAbilityFlaw in ancestry.AbilityFlaws )
        {
            AbilityScores.GetCharacteristic( ancestryAbilityFlaw )
               .Value -= 2;
        }
    }
}
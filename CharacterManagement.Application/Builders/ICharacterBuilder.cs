using System.Collections;
using System.Collections.Generic;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Application.Builders;

public interface ICharacterBuilder
{
    public void SetAncestry( AncestryType ancestryType );
    public void SetBackground();
    public void SetClass();
    public void IncreaseAbilityScores( IEnumerable<AbilityType> increasedAbilityScores );
    public void SetInventory();
    public void SetAlignment();
    public void SetDeity();
    public void SetAge();
    public void SetGender();
    public void SetName( string name );


}
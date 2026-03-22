using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Builders;

public interface ICharacterBuilder
{
    void CreateCharacter(
        int userId,
        string name,
        int raceId );

    void SetAncestry( AncestryType ancestryType );
    void SetBackground();
    void SetClass();
    void IncreaseAbilityScores( IEnumerable<AbilityType> increasedAbilityScores );
    void ApplyFreeBoosts( IEnumerable<AbilityType> freeBoosts );
    void SetInventory();
    void SetAlignment();
    void SetDeity();
    void SetAge();
    void SetGender();
    void SetName( string name );
    DraftCharacter Build();
}
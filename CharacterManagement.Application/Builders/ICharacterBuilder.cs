using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.Builders;

public interface ICharacterBuilder
{
    void CreateCharacter(
        int accountId,
        string name,
        AncestryType ancestryType,
        string? concept = null,
        int? age = null );

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

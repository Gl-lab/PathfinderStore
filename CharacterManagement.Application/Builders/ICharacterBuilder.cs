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
    void SetAncestryPackage( string heritageId, string ancestryFeatId );
    void SetBackground(
        string backgroundId,
        AbilityType restrictedBoost,
        AbilityType freeBoost,
        IReadOnlyList<BackgroundTrainingChoice>? trainingChoices = null );
    void SetClass(
        string characterClassId,
        AbilityType keyAbility,
        string? rogueRacketId = null,
        IReadOnlyList<RogueTrainingChoice>? rogueTrainingChoices = null,
        string? clericDoctrineId = null,
        string? deityId = null,
        DivineFont? divineFont = null,
        DivineSanctification? divineSanctification = null,
        string? deitySkillReplacementId = null,
        string? huntersEdgeId = null,
        string? druidicOrderId = null,
        string? bardMuseId = null,
        string? witchPatronId = null,
        string? witchPatronFamiliarSpellId = null,
        string? arcaneSchoolId = null );
    void SetFinalFreeBoosts( IReadOnlyList<AbilityType> finalFreeBoosts );
    void SetClassTraining(
        string characterClassId,
        IReadOnlyList<ClassSkillGrantChoice> grantChoices,
        IReadOnlyList<ClassTrainingTargetChoice> additionalChoices );
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

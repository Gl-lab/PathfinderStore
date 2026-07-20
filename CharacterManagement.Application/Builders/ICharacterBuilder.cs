using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Training;
using Pathfinder.CharacterManagement.Domain.Rules.Feats;

namespace Pathfinder.CharacterManagement.Application.Builders;

public interface ICharacterBuilder
{
    void CreateCharacter(
        int accountId,
        string name,
        AncestryType ancestryType,
        string? concept = null,
        int? age = null,
        CharacterGender gender = CharacterGender.NotSpecified,
        AvatarId? avatarId = null );

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
        string? arcaneSchoolId = null,
        string? arcaneThesisId = null,
        string? clericDomainId = null,
        IReadOnlyList<string>? clericCantripIds = null,
        IReadOnlyList<string>? clericPreparedSpellIds = null,
        IReadOnlyList<string>? bardCantripIds = null,
        IReadOnlyList<string>? bardSpellIds = null,
        IReadOnlyList<string>? druidCantripIds = null,
        IReadOnlyList<string>? druidPreparedSpellIds = null,
        IReadOnlyList<string>? witchFamiliarCantripIds = null,
        IReadOnlyList<string>? witchFamiliarSpellIds = null,
        IReadOnlyList<string>? witchPreparedCantripIds = null,
        IReadOnlyList<string>? witchPreparedSpellIds = null,
        string? witchFocusHexId = null,
        IReadOnlyList<string>? wizardSpellbookCantripIds = null,
        IReadOnlyList<string>? wizardSpellbookSpellIds = null,
        string? wizardCurriculumCantripId = null,
        IReadOnlyList<string>? wizardCurriculumSpellIds = null,
        IReadOnlyList<string>? wizardPreparedCantripIds = null,
        IReadOnlyList<string>? wizardPreparedSpellIds = null,
        string? wizardPreparedCurriculumCantripId = null,
        string? wizardPreparedCurriculumSpellId = null,
        IReadOnlyList<FeatChoice>? classFeatChoices = null );
    void SetFinalFreeBoosts( IReadOnlyList<AbilityType> finalFreeBoosts );
    void SetAdditionalLanguages( IReadOnlyList<string> additionalLanguageIds );
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
    void SetGender( CharacterGender gender );
    void SetName( string name );
    DraftCharacter Build();
}

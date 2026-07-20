using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Training;

namespace Pathfinder.CharacterManagement.Application.DTO;

public class CreateCharacterRequestDto
{
    public string Name { get; set; }
    public string? Concept { get; set; }
    public int? Age { get; set; }
    public CharacterGender Gender { get; set; }
    public AncestryType AncestryType { get; set; }
    public string HeritageId { get; set; }
    public string AncestryFeatId { get; set; }
    public IReadOnlyList<AbilityType> FreeBoosts { get; set; } = [];
    public string BackgroundId { get; set; } = String.Empty;
    public AbilityType? BackgroundRestrictedBoost { get; set; }
    public AbilityType? BackgroundFreeBoost { get; set; }
    public IReadOnlyList<BackgroundTrainingChoice> BackgroundTrainingChoices { get; set; } = [];
    public string ClassId { get; set; } = String.Empty;
    public AbilityType? ClassKeyAbility { get; set; }
    public string? RogueRacketId { get; set; }
    public IReadOnlyList<RogueTrainingChoice> RogueTrainingChoices { get; set; } = [];
    public string? HuntersEdgeId { get; set; }
    public string? DruidicOrderId { get; set; }
    public string? BardMuseId { get; set; }
    public string? WitchPatronId { get; set; }
    public string? WitchPatronFamiliarSpellId { get; set; }
    public string? ArcaneSchoolId { get; set; }
    public string? ArcaneThesisId { get; set; }
    public string? ClericDoctrineId { get; set; }
    public string? DeityId { get; set; }
    public string? ClericDomainId { get; set; }
    public DivineFont? DivineFont { get; set; }
    public DivineSanctification? DivineSanctification { get; set; }
    public string? DeitySkillReplacementId { get; set; }
    public IReadOnlyList<string> ClericCantripIds { get; set; } = [];
    public IReadOnlyList<string> ClericPreparedSpellIds { get; set; } = [];
    public IReadOnlyList<string> BardCantripIds { get; set; } = [];
    public IReadOnlyList<string> BardSpellIds { get; set; } = [];
    public IReadOnlyList<string> DruidCantripIds { get; set; } = [];
    public IReadOnlyList<string> DruidPreparedSpellIds { get; set; } = [];
    public IReadOnlyList<string> WitchFamiliarCantripIds { get; set; } = [];
    public IReadOnlyList<string> WitchFamiliarSpellIds { get; set; } = [];
    public IReadOnlyList<string> WitchPreparedCantripIds { get; set; } = [];
    public IReadOnlyList<string> WitchPreparedSpellIds { get; set; } = [];
    public string? WitchFocusHexId { get; set; }
    public IReadOnlyList<string> WizardSpellbookCantripIds { get; set; } = [];
    public IReadOnlyList<string> WizardSpellbookSpellIds { get; set; } = [];
    public string? WizardCurriculumCantripId { get; set; }
    public IReadOnlyList<string> WizardCurriculumSpellIds { get; set; } = [];
    public IReadOnlyList<string> WizardPreparedCantripIds { get; set; } = [];
    public IReadOnlyList<string> WizardPreparedSpellIds { get; set; } = [];
    public string? WizardPreparedCurriculumCantripId { get; set; }
    public string? WizardPreparedCurriculumSpellId { get; set; }
    public IReadOnlyList<AbilityType>? FinalFreeBoosts { get; set; }
    public IReadOnlyList<ClassSkillGrantChoice> ClassSkillGrantChoices { get; set; } = [];
    public IReadOnlyList<ClassTrainingTargetChoice> AdditionalClassTrainingChoices { get; set; } = [];
}

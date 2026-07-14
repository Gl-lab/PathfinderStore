using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public class CreateCharacterRequestDto
{
    public string Name { get; set; }
    public string? Concept { get; set; }
    public int? Age { get; set; }
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
    public string? ClericDoctrineId { get; set; }
    public string? DeityId { get; set; }
    public DivineFont? DivineFont { get; set; }
    public DivineSanctification? DivineSanctification { get; set; }
    public string? DeitySkillReplacementId { get; set; }
    public IReadOnlyList<AbilityType>? FinalFreeBoosts { get; set; }
    public IReadOnlyList<ClassSkillGrantChoice> ClassSkillGrantChoices { get; set; } = [];
    public IReadOnlyList<ClassTrainingTargetChoice> AdditionalClassTrainingChoices { get; set; } = [];
}

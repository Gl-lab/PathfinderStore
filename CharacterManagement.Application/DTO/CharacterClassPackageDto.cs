using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class CharacterClassPackageDto
{
    public string ClassId { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public int BaseHitPoints { get; set; }
    public AbilityType KeyAbility { get; set; }
    public int AdditionalSkillCount { get; set; }
    public SpellTradition? SpellTradition { get; set; }
    public RogueRacketPackageDto? RogueRacket { get; set; }
    public HuntersEdgePackageDto? HuntersEdge { get; set; }
    public DruidicOrderPackageDto? DruidicOrder { get; set; }
    public BardMusePackageDto? BardMuse { get; set; }
    public WitchPatronPackageDto? WitchPatron { get; set; }
    public ArcaneSchoolPackageDto? ArcaneSchool { get; set; }
    public ArcaneThesisPackageDto? ArcaneThesis { get; set; }
    public ClericDoctrinePackageDto? ClericDoctrine { get; set; }
    public DeityPackageDto? Deity { get; set; }
    public ClericDomainPackageDto? ClericDomain { get; set; }
    public ClericSpellLoadoutDto? ClericSpellLoadout { get; set; }
    public IReadOnlyList<CharacterClassRuleDto> Rules { get; set; } = [];
}

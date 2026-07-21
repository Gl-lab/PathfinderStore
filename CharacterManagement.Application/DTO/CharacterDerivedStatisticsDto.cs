using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Statistics;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class CharacterDerivedStatisticsDto
{
    public CharacterHitPointsDto HitPoints { get; set; } = new CharacterHitPointsDto();
    public CharacterArmorClassDto ArmorClass { get; set; } = new CharacterArmorClassDto();
    public CharacterProficiencyStatisticDto Perception { get; set; } = new CharacterProficiencyStatisticDto();
    public CharacterSavingThrowsDto SavingThrows { get; set; } = new CharacterSavingThrowsDto();
    public CharacterSkillModifiersDto SkillModifiers { get; set; } = new CharacterSkillModifiersDto();
}

public sealed class CharacterArmorClassDto
{
    public int Base { get; set; }
    public AbilityType Ability { get; set; }
    public int AbilityModifier { get; set; }
    public int? AbilityCap { get; set; }
    public int AppliedAbilityModifier { get; set; }
    public string ProficiencyTargetId { get; set; } = String.Empty;
    public ProficiencyRank ProficiencyRank { get; set; }
    public int ProficiencyBonus { get; set; }
    public IReadOnlyList<string> ProficiencySourceGrantIds { get; set; } = [];
    public IReadOnlyList<CharacterStatisticBonusDto> ItemBonuses { get; set; } = [];
    public IReadOnlyList<CharacterStatisticBonusDto> StatusBonuses { get; set; } = [];
    public IReadOnlyList<CharacterStatisticBonusDto> CircumstanceBonuses { get; set; } = [];
    public int Total { get; set; }
}

public sealed class CharacterStatisticBonusDto
{
    public string SourceId { get; set; } = String.Empty;
    public StatisticBonusType Type { get; set; }
    public int Value { get; set; }
}

public sealed class CharacterHitPointsDto
{
    public int Maximum { get; set; }
    public int Ancestry { get; set; }
    public int Class { get; set; }
    public int ConstitutionModifier { get; set; }
}

public sealed class CharacterSavingThrowsDto
{
    public CharacterProficiencyStatisticDto Fortitude { get; set; } = new CharacterProficiencyStatisticDto();
    public CharacterProficiencyStatisticDto Reflex { get; set; } = new CharacterProficiencyStatisticDto();
    public CharacterProficiencyStatisticDto Will { get; set; } = new CharacterProficiencyStatisticDto();
}

public sealed class CharacterSkillModifiersDto
{
    public IReadOnlyList<CharacterProficiencyStatisticDto> General { get; set; } = [];
    public IReadOnlyList<CharacterProficiencyStatisticDto> Lore { get; set; } = [];
}

public sealed class CharacterProficiencyStatisticDto
{
    public string TargetId { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public AbilityType Ability { get; set; }
    public int AbilityModifier { get; set; }
    public ProficiencyRank ProficiencyRank { get; set; }
    public int ProficiencyBonus { get; set; }
    public int Total { get; set; }
    public IReadOnlyList<string> SourceGrantIds { get; set; } = [];
}

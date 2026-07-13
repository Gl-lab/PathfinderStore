namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class CharacterDerivedStatisticsDto
{
    public CharacterHitPointsDto HitPoints { get; set; } = new CharacterHitPointsDto();
}

public sealed class CharacterHitPointsDto
{
    public int Maximum { get; set; }
    public int Ancestry { get; set; }
    public int Class { get; set; }
    public int ConstitutionModifier { get; set; }
}

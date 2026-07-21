using Pathfinder.CharacterManagement.Domain.Rules.Statistics;

namespace Pathfinder.CharacterManagement.Application.DTO;

public sealed class ChangeHitPointsRequestDto
{
    public HitPointOperation Operation { get; set; }
    public int Amount { get; set; }
}

public sealed class CharacterHitPointStateDto
{
    public int Current { get; set; }
    public int Temporary { get; set; }
    public int Maximum { get; set; }
}

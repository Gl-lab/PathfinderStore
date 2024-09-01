using Pathfinder.CharacterManagement.Application.DTO.Base;

namespace Pathfinder.CharacterManagement.Application.DTO;

public class RaceDto: BaseDto
{
    public string Name { get; set; }
    public int SizeId { get; set; }
    public virtual RaceSizeDto Size { get; set; }
    public int BaseSpeed { get; set;}
    public bool IsNightVision { get; set; }
}
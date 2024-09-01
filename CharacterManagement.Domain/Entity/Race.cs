namespace Pathfinder.CharacterManagement.Domain.Entity;

public class Race: Pathfinder.Utils.Entities.Base.Entity
{
    public string Name { get; set; }
    public RaceSizeType SizeId { get; set; }
    //public virtual RaceSize Size { get; set; }
    public int BaseSpeed { get; set;}
    public bool IsNightVision { get; set; }
}
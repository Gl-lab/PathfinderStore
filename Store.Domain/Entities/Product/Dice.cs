using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Store.Domain.Entities.Product;

public class Dice: Entity
{
    public int Count { get; set; }
    public DiceType D { get; set; }
}
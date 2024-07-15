using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Core.Entities.Product
{
    public class Dice: Entity
    {
        public int Count { get; set; }
        public DiceType D { get; set; }
    }
}
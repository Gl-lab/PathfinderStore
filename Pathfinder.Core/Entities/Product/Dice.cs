using Pathfinder.Core.Entities.Base;
namespace Pathfinder.Core.Entities.Product
{
    public class Dices: Entity
    {
        public int Count { get; set; }
        public virtual BaseDice D { get; set; }
    }
}
using Pathfinder.Core.Entities.Base;

namespace Pathfinder.Core.Entities.Product
{
    public class Item : Entity
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
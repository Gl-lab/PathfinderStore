using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
using Pathfinder.Core.Entities.Product;
namespace Pathfinder.Core.Entities.Shop
{
    public class ShopsProduct: Entity
    {
        public virtual Shop Shop { get; set; }
        public virtual Item Item { get; set; }
        public int Count { get; set; }
    }
}
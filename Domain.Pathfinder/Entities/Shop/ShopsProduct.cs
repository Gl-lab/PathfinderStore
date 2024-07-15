using System.Collections.Generic;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Core.Entities.Shop
{
    public class ShopsProduct: Entity
    {
        public virtual Shop Shop { get; set; }
        public virtual Item Item { get; set; }
        public short Count { get; set; }
    }
}
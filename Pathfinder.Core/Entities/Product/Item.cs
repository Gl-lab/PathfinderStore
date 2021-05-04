using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Core.Entities.Product
{
    public class Item: Entity
    {
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
    }
}
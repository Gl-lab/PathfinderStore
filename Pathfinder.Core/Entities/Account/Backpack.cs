using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
using Pathfinder.Core.Entities.Product;
namespace Pathfinder.Core.Entities.Account
{
    public class Backpack: Entity
    {
        public virtual Character Character { get; set; }
        public virtual Item Item { get; set; }
        public int Count { get; set; }
        public bool IsWearable { get; set; }
    }
}
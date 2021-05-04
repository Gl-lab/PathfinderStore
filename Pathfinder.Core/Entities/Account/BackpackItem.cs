using Pathfinder.Core.Entities.Base;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Core.Entities.Account
{
    public class BackpackItem: Entity
    {
        /*public int CharacterId { get; set; }
        public virtual Character Character { get; set; }*/
        public virtual Item Item { get; set; }
        public bool IsWearing { get; set; }
        public short Count { get; set; }
    }
}
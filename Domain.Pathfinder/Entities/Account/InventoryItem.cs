using Pathfinder.Core.Entities.Product;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Core.Entities.Account
{
    public class InventoryItem: Entity
    {
        /*public int CharacterId { get; set; }
        public virtual Character Character { get; set; }*/
        public virtual Item Item { get; set; }
      //  public bool IsWearing { get; set; }
        public short Count { get; set; }
    }
}
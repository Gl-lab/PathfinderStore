using System.Collections.Generic;
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Core.Entities.Account
{
    public class Inventory: Entity
    {
       public Wallet Wallet { get; init; }
       public ICollection<InventoryItem> Items { get; init; } = new List<InventoryItem>();
    }
}
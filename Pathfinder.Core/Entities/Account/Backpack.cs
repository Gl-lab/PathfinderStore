using System.Collections;
using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
using Pathfinder.Core.Entities.Product;
namespace Pathfinder.Core.Entities.Account
{
    public class Backpack: Entity
    {
       public virtual Wallet Wallet { get; init; }
       public virtual ICollection<BackpackItem> Items { get; init; } = new List<BackpackItem>();
    }
}
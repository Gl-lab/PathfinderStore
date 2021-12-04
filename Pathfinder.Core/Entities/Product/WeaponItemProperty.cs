using System.Collections.Generic;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Base;

namespace Pathfinder.Core.Entities.Product
{
    public class WeaponItemProperty: Entity
    {
        public int ItemId { get; set; }
        public virtual Item Item { get; set; }
        public bool IsMasterful { get ;set; }
        public SizeType Size  { get ; set; }
        public virtual ICollection<AdditionalDamage> AdditionalDamages { get ; set; } = new List<AdditionalDamage>();
    }
}
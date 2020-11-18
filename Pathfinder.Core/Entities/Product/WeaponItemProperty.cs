using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Core.Entities.Product
{
    public class WeaponItemProperty: Entity
    {
        public virtual Item Weapon { get; set; }
        public bool IsMasterful { get ;set; }
        public Size Size  { get ; set; }
    }
}
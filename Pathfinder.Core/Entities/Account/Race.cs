using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
namespace Pathfinder.Core.Entities.Account
{
    public class Race: Entity
    {
        public string Name { get; set; }
        public SizeType SizeId { get; set; }
        public virtual Size Size { get; set; }
        public int BaseSpeed { get; set;}
        public bool IsNightVision { get; set; }
    }
}
using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
using Pathfinder.Core.Entities.Product;
namespace Pathfinder.Core.Entities.Account
{
    public class Character: Entity
    {
        public string Name { get; set; }
        public int Balance { get; set; }
        public int RaceId { get; set; }
        public virtual ICollection<Backpack> Items { get; set; } = new List<Backpack>();
        public virtual Race Race { get; set; }
        public virtual GroupCharacteristic Characteristics { get; set; }
    }
}
using System.Collections.Generic;
using Pathfinder.Core.Entities.Base;
namespace Pathfinder.Core.Entities.Account
{
    public class GroupCharacteristic: Entity
    {
        public virtual Characteristic Strength { get; set; }
        public virtual Characteristic Dexterity { get; set; }
        public virtual Characteristic Constitution { get; set; }
        public virtual Characteristic Intelligence { get; set; }
        public virtual Characteristic Wisdom { get; set; }
        public virtual Characteristic Charisma { get; set; }
    }
}
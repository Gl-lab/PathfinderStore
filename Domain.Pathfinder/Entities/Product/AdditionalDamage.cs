using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Core.Entities.Product
{
    public class AdditionalDamage: Entity
    {
        public virtual Dice Dice { get; set; } 
        public virtual DamageType DamageType { get; set; }
    }
}
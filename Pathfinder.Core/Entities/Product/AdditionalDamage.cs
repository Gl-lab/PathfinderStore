using Pathfinder.Core.Entities.Base;

namespace Pathfinder.Core.Entities.Product
{
    public class AdditionalDamage: Entity
    {
        public virtual Dices Dices { get; set; } 
        public virtual DamageType DamageType { get; set; }
    }
}
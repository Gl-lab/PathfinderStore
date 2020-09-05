using Pathfinder.Core.Entities.Base;
using System.Collections.Generic;
namespace Pathfinder.Core.Entities
{
    public class Weapon: Entity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int CountDiceDamage { get; set; }
        public BaseDice SizeDiceDamage { get; set; }
        public int? Range { get; set; }
        public Ammunition Ammunition { get; set; }
        public int MultiplierCrit { get; set; }
        public int CritRange { get; set; }
        public List<DamageType> DamageTypeList { get; set; } = new List<DamageType>();
        public WeaponType WeaponType { get; set; }
    }
}
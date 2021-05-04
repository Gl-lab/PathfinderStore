using System.Collections.Generic;
using Pathfinder.Application.DTO.Base;
using Pathfinder.Core.Entities.Account;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.DTO.Items
{
    public class WeaponItemDto: BaseDto
    {
        public virtual ItemDto Item { get; set; }
        public bool IsMasterful { get ;set; }
        public SizeType Size  { get ; set; }
        public Dices Damage { get; set; }
        public ICollection<AdditionalDamage> AdditionalDamages{ get; set; }  = new List<AdditionalDamage>();
        public int? Range { get; set; }
        public int MultiplierCrit { get; set; }
        public int CritRange { get; set; }
        public Ammunition Ammunition { get; set; }
        public ICollection<DamageType> DamageTypeList { get; set; } = new List<DamageType>();
        public WeaponType WeaponType { get; set; }
    }
}
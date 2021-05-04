using System;
using Pathfinder.Core.Entities.Base;
using System.Collections.Generic;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Core.Entities.Product
{
    public class Weapon: Entity
    {
        public int ArticleId { get; set; }
        public int? Range { get; set; }
        public int MultiplierCrit { get; set; }
        public int CritRange { get; set; }
        public virtual Ammunition Ammunition { get; set; }
        public virtual ICollection<DamageType> DamageTypeList { get; set; } = new List<DamageType>();
        public virtual WeaponType WeaponType { get; set; }
        public virtual Dices SmallSizeDamage { get; set; }
        public virtual Dices MediumSizeDamage { get; set; }
        public virtual Article Article { get; set; }

        public Dices DamageBySize(SizeType size)
        {
            return size switch
            {
                SizeType.Medium => MediumSizeDamage,
                SizeType.Small => SmallSizeDamage,
                _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
            };
        }
    }
}
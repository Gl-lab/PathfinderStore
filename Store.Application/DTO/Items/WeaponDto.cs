using System.Collections.Generic;
using Pathfinder.Store.Application.DTO.Base;

namespace Pathfinder.Store.Application.DTO.Items;

public class WeaponDto : BaseDto
{
    public int ArticleId { get; set; }
    public int? Range { get; set; }
    public int MultiplierCrit { get; set; }
    public int CritRange { get; set; }
    public Ammunition Ammunition { get; set; }
    public ICollection<DamageType> DamageTypeList { get; set; } = new List<DamageType>();
    public WeaponType WeaponType { get; set; }
    public DicesDto SmallSizeDamage { get; set; }
    public DicesDto MediumSizeDamage { get; set; }
    public DicesDto LargeSizeDamage { get; set; }
    public ProductDto Product { get; set; }
}
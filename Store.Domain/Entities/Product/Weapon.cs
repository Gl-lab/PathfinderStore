using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Store.Domain.Entities.Product;

public class Weapon : Entity
{
    public int ProductId { get; set; }
    public int? Range { get; set; }
    public int MultiplierCrit { get; set; }
    public int CritRange { get; set; }
    public Ammunition Ammunition { get; set; }
    public List<DamageType> DamageTypeList { get; set; } = new List<DamageType>();
    public WeaponType WeaponType { get; set; }
    public Dice SmallSizeDamage { get; set; }
    public Dice MediumSizeDamage { get; set; }
    public Product Product { get; set; }

    public Dice DamageBySize(SizeType size)
    {
        return size switch
               {
                   SizeType.Medium => MediumSizeDamage,
                   SizeType.Small => SmallSizeDamage,
                   _ => throw new ArgumentOutOfRangeException(nameof(size), size, "Unknown SizeType")
               };
    }
}
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Store.Domain.Entities.Product;

public class WeaponItemProperty: Entity
{
    public int ItemId { get; set; }
    public virtual Item Item { get; set; }
    public bool IsMasterful { get ;set; }
    public SizeType Size  { get ; set; }
    public virtual ICollection<AdditionalDamage> AdditionalDamages { get ; set; } = new List<AdditionalDamage>();
}
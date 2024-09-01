using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Store.Domain.Entities.Product;

public class WeaponType : Entity
{
    public string Name { get; set; }
    public string ShortName { get; set; }
    public WeaponProficiencyType WeaponProficiencyId { get; set; }
    public virtual WeaponProficiency WeaponProficiency { get; set; }
}
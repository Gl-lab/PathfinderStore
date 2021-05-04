using Pathfinder.Core.Entities.Base;
namespace Pathfinder.Core.Entities.Product
{
    public class WeaponType : Entity
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public WeaponProficiencyType WeaponProficiencyId { get; set; }
        public virtual WeaponProficiency WeaponProficiency { get; set; }
    }
}
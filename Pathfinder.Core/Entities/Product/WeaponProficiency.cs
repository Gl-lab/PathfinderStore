using System.ComponentModel.DataAnnotations;

namespace Pathfinder.Core.Entities.Product
{
    public class WeaponProficiency
    {
        [Key]
        public WeaponProficiencyType Id { get; set; }
        [Required, StringLength(80)]
        public string Name { get; set; }
    }
    
    public enum WeaponProficiencyType: byte
    {
        Simple,
        Martial,
        Exotic 
    }
}
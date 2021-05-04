using Pathfinder.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Pathfinder.Core.Entities.Product
{
    public class Category
    {
        [Key]
        public CategoryType CategoryType { get; set; }
        [Required, StringLength(80)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
    
    public enum CategoryType: byte
    {
        Weapon,
        Armor
    }
}

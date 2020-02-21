using Pathfinder.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Pathfinder.Core.Entities
{
    public class Category : Entity
    {
        [Required, StringLength(80)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

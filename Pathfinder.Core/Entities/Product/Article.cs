using Pathfinder.Core.Entities.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pathfinder.Core.Entities.Product
{
    public class Article : Entity
    {
        [Required, StringLength(80)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageFile { get; set; }
        public decimal? Price { get; set; }
        public decimal? Weight { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Effect> Effects { get; set; } = new List<Effect>();
    }
}

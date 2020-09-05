﻿using Pathfinder.Core.Entities.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pathfinder.Core.Entities
{
    public class Product : Entity
    {
        [Required, StringLength(80)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageFile { get; set; }
        public decimal? Price { get; set; }

        // n-1 relationships
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}

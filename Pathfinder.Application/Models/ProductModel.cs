using Pathfinder.Application.Models.Base;
using System;

namespace Pathfinder.Application.Models
{
    public class ProductModel : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; }
    }
}

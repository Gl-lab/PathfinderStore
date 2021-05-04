using Pathfinder.Application.DTO.Base;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.DTO
{
    public class ArticleDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Weight { get; set; }
        public byte CategoryType { get; set; }
        public CategoryDto Category { get; set; }
    }
}
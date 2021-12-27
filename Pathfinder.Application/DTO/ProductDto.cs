using Pathfinder.Application.DTO.Base;

namespace Pathfinder.Application.DTO
{
    public class ProductDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Weight { get; set; }
        public byte CategoryType { get; set; }
        public CategoryDto Category { get; set; }
    }
}
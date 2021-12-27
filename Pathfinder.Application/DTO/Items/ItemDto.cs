using Pathfinder.Application.DTO.Base;

namespace Pathfinder.Application.DTO.Items
{
    public class ItemDto : BaseDto
    {
        public int ArticleId { get; set; }
        public ProductDto Product { get; set; }
    }
}
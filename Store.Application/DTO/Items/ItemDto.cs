using Pathfinder.Store.Application.DTO.Base;

namespace Pathfinder.Store.Application.DTO.Items;

public class ItemDto : BaseDto
{
    public int ArticleId { get; set; }
    public ProductDto Product { get; set; }
}
using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Store.Domain.Entities.Product;

public class Item : Entity
{
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
}
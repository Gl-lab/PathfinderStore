using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Store.Domain.Entities.Shop;

public class Shop : Entity
{
    public string Name { get; set; }
    public string Description { get; set; }
}

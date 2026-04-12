using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Store.Domain.Entities.Product;

public class Effect: Entity
{
    public string Name { get; set; }
    public string Description {get; set; }
}
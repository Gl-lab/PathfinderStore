using Pathfinder.Utils.Entities.Base;

namespace Pathfinder.Store.Domain.Entities.Product;

public class Product : Entity
{
        
    // public string Name { get; set; }
    // public string Description { get; set; }
    // public string ImageFile { get; set; }
       
    public decimal? Price { get; set; }
    public decimal? Weight { get; set; }

    public CategoryType CategoryType { get; set; }
    //   public virtual Category Category { get; set; }
    //public List<Effect> Effects { get; set; } = new List<Effect>();
}
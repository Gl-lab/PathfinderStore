namespace Pathfinder.Store.Domain.Entities.Product;

public class Category
{
    public CategoryType CategoryType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
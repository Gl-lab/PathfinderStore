using System.Threading.Tasks;
using MediatR;

namespace Pathfinder.Application.UseCases.Products;

public class UpdateArticleCommand : IRequest<Task>
{
    public UpdateArticleCommand(int id, string name, string description, decimal? price, decimal? weight,
        byte categoryType)
    {
        Name = name;
        Description = description;
        Price = price;
        Weight = weight;
        CategoryType = categoryType;
        Id = id;
    }

    public int Id { get; }
    public string Name { get; }
    public string Description { get; }
    public decimal? Price { get; set; }
    public decimal? Weight { get; set; }
    public byte CategoryType { get; set; }
}
using MediatR;
using Pathfinder.Store.Application.DTO;

namespace Pathfinder.Store.Application.UseCases.Products;

public class CreateArticleCommand : IRequest<ProductDto>
{
    public string Name { get; }
    public string Description { get; }
    public decimal? Price { get; }
    public decimal? Weight { get; }
    public byte CategoryType { get; }

    public CreateArticleCommand(string name, string description, decimal? price, decimal? weight, byte categoryType)
    {
        Name = name;
        Description = description;
        Price = price;
        Weight = weight;
        CategoryType = categoryType;
    }
}
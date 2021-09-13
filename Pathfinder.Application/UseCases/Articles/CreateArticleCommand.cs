using MediatR;
using Pathfinder.Application.DTO;

namespace Pathfinder.Application.UseCases.Articles
{
    public class CreateArticleCommand: IRequest<ArticleDto>
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
}
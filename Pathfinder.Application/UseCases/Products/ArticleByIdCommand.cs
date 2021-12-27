using MediatR;
using Pathfinder.Application.DTO;

namespace Pathfinder.Application.UseCases.Products;

public class ArticleByIdCommand : IRequest<ProductDto>
{
    public ArticleByIdCommand(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
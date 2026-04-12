using MediatR;
using Pathfinder.Store.Application.DTO;

namespace Pathfinder.Store.Application.UseCases.Products;

public class ArticleByIdCommand : IRequest<ProductDto>
{
    public ArticleByIdCommand(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
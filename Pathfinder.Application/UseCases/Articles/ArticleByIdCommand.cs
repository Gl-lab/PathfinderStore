using MediatR;
using Pathfinder.Application.DTO;

namespace Pathfinder.Application.UseCases.Articles;

public class ArticleByIdCommand: IRequest<ArticleDto>
{
    public ArticleByIdCommand(int id)
    {
        Id = id;
    }
    public int Id { get; }
}
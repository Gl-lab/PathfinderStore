using System.Threading.Tasks;
using MediatR;

namespace Pathfinder.Application.UseCases.Articles;

public class DeleteArticleCommand: IRequest<Task>
{
    public DeleteArticleCommand(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
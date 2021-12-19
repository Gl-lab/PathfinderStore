using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.UnitOfWork;

namespace Pathfinder.Application.UseCases.Articles;

public class DeleteArticleHandler: IRequestHandler<DeleteArticleCommand, Task>
{
    private readonly IProductService _productService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteArticleHandler(IProductService productService, IUnitOfWork unitOfWork)
    {
        _productService = productService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Task> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _productService.GetArticleById(request.Id);
        if (article == null) return Task.CompletedTask;
        await _productService.DeleteArticle(article);
        await _unitOfWork.CommitAsync();
        return Task.CompletedTask;
    }
}
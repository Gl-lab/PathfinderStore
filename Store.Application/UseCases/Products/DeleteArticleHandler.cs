using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Store.Application.Services;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.Store.Application.UseCases.Products;

public class DeleteArticleHandler : IRequestHandler<DeleteArticleCommand, Task>
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
        Product article = await _productService.GetArticleById(request.Id);
        if (article == null)
        {
            return Task.CompletedTask;
        }

        await _productService.DeleteArticle(article);
        await _unitOfWork.Commit();
        return Task.CompletedTask;
    }
}
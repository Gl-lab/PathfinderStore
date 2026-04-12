using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Store.Application.Exceptions;
using Pathfinder.Store.Application.Services;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.Store.Application.UseCases.Products;

public class UpdateArticleHandler : IRequestHandler<UpdateArticleCommand, Task>
{
    private readonly IProductService _productService;
    private readonly IUnitOfWork _unitOfWork;


    public UpdateArticleHandler( IProductService productService, IUnitOfWork unitOfWork )
    {
        _productService = productService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Task> Handle( UpdateArticleCommand request, CancellationToken cancellationToken )
    {
        if ( request.Id < 1 )
        {
            throw new PathfinderApplicationException( $"Incorrect Id={request.Id} in {nameof( UpdateArticleHandler )}" );
        }

        if ( !Enum.IsDefined( typeof( CategoryType ), request.CategoryType ) )
        {
            throw new PathfinderApplicationException(
                $"Incorrect CategoryType={request.CategoryType} in {nameof( CategoryType )}" );
        }

        Product article = await _productService.GetArticleById( request.Id );
        article.CategoryType = ( CategoryType )request.CategoryType;
        // article.Name = request.Name;
        // article.Description = request.Description;
        article.Price = article.Price;
        article.Weight = article.Weight;

        await _productService.UpdateArticle( article );
        return _unitOfWork.Commit();
    }
}
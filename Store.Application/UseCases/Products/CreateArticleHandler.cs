using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Store.Application.DTO;
using Pathfinder.Store.Application.Services;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.Store.Application.UseCases.Products;

public class CreateArticleHandler : IRequestHandler<CreateArticleCommand, ProductDto>
{
    private readonly IProductService _productService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateArticleHandler( IProductService productService, IUnitOfWork unitOfWork )
    {
        _productService = productService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductDto> Handle( CreateArticleCommand request, CancellationToken cancellationToken )
    {
        Product product;
        try
        {
            product = await _productService.CreateArticle( request.Name, request.Description, request.Price,
                request.Weight,
                request.CategoryType );
            await _unitOfWork.Commit();
        }
        catch ( Exception )
        {
            await _unitOfWork.Rollback();
            throw;
        }

        //return _mapper.Map<ProductDto>( product );
        throw new NotImplementedException();
    }
}
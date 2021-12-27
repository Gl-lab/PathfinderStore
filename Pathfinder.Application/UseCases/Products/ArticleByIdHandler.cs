using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Interfaces;

namespace Pathfinder.Application.UseCases.Products;

public class ArticleByIdHandler : IRequestHandler<ArticleByIdCommand, ProductDto>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ArticleByIdHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(ArticleByIdCommand request, CancellationToken cancellationToken)
    {
        var article = await _productService.GetArticleById(request.Id);
        return _mapper.Map<ProductDto>(article);
    }
}
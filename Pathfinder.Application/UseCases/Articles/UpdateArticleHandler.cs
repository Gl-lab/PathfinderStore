using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.UnitOfWork;
using Pathfinder.Application.Exceptions;

namespace Pathfinder.Application.UseCases.Articles;

public class UpdateArticleHandler: IRequestHandler<UpdateArticleCommand, Task>
{
    private readonly IProductService _productService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    

    public UpdateArticleHandler(IProductService productService, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _productService = productService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Task> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        
        if (request.Id < 1)
        {
            throw new PathfiderApplicationException($"Incorrect Id={request.Id} in {nameof(UpdateArticleHandler)}");
        }
        
        if (!Enum.IsDefined(typeof(CategoryType), request.CategoryType))
        {
            throw new PathfiderApplicationException($"Incorrect CategoryType={request.CategoryType} in {nameof(CategoryType)}");
        }

        var article = await _productService.GetArticleById(request.Id);
        article.CategoryType = (CategoryType)request.CategoryType;
        article.Name = request.Name;
        article.Description = request.Description;
        article.Price = article.Price;
        article.Weight = article.Weight;
        
        await _productService.UpdateArticle(article);
        return _unitOfWork.CommitAsync();
    }
}
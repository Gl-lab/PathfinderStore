using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Exceptions;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.UseCases.Category;

public class CategoryByCategoryTypeHandler : IRequestHandler<CategoryByCategoryTypeCommand, CategoryDto>
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public CategoryByCategoryTypeHandler(ICategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(CategoryByCategoryTypeCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(CategoryType), request.CategoryType))
        {
            throw new PathfiderApplicationException(
                $"Incorrect CategoryType={request.CategoryType} in {nameof(CategoryType)}");
        }

        var category = await _categoryService.Get((CategoryType)request.CategoryType);
        return _mapper.Map<CategoryDto>(category);
    }
}
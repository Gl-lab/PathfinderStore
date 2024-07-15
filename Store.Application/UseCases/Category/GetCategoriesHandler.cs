using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Services;

namespace Pathfinder.Application.UseCases.Category;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesCommand, ICollection<CategoryDto>>
{
    private readonly ICategoryService _categoryService;

    public GetCategoriesHandler( ICategoryService categoryService )
    {
        _categoryService = categoryService;
    }

    public async Task<ICollection<CategoryDto>> Handle( GetCategoriesCommand request,
                                                        CancellationToken cancellationToken )
    {
        ICollection<Core.Entities.Product.Category> categories = await _categoryService.GetCategoryList();
        throw new NotImplementedException();
        //return _mapper.Map<ICollection<CategoryDto>>( categories );
    }
}
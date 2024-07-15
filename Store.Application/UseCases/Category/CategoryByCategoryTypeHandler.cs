using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Exceptions;
using Pathfinder.Application.Services;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.UseCases.Category;

public class CategoryByCategoryTypeHandler : IRequestHandler<CategoryByCategoryTypeCommand, CategoryDto>
{
    private readonly ICategoryService _categoryService;

    public CategoryByCategoryTypeHandler( ICategoryService categoryService )
    {
        _categoryService = categoryService;
    }

    public async Task<CategoryDto> Handle( CategoryByCategoryTypeCommand request, CancellationToken cancellationToken )
    {
        if ( !Enum.IsDefined( typeof( CategoryType ), request.CategoryType ) )
        {
            throw new PathfiderApplicationException(
                $"Incorrect CategoryType={request.CategoryType} in {nameof( CategoryType )}" );
        }

        Core.Entities.Product.Category category = await _categoryService.Get( ( CategoryType )request.CategoryType );
        throw new NotImplementedException();
        //return _mapper.Map<CategoryDto>( category );
    }
}
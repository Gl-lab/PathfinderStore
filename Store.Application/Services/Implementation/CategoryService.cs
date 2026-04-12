using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Store.Application.Repositories;

namespace Pathfinder.Store.Application.Services.Implementation;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService( ICategoryRepository categoryRepository )
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException( nameof( categoryRepository ) );
    }

    public async Task<Category> Get( CategoryType categoryType )
    {
        Category category = await _categoryRepository
                                 .GetAsync( categoryType )
                                 .ConfigureAwait( false );
        return category;
    }

    public async Task<ICollection<Category>> GetCategoryList()
    {
        ICollection<Category> categoryList = await _categoryRepository.ListAsync();
        return categoryList;
    }
}
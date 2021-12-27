using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Repositories;

namespace Pathfinder.Application.Services
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<Category> Get(CategoryType categoryType)
        {
            var category = await _categoryRepository
                .GetAsync(categoryType)
                .ConfigureAwait(false);
            return category;
        }

        public async Task<ICollection<Category>> GetCategoryList()
        {
            var categoryList = await _categoryRepository.ListAsync();
            return categoryList;
        }
    }
}
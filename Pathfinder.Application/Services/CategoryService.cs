using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.DTO;
using Pathfinder.Core.Repositories;
using AutoMapper;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
       // private readonly IAppLogger<CategoryService> logger;
        private readonly IMapper mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CategoryDto> Get(CategoryType categoryType)
        {
            var category = await categoryRepository
                .GetAsync(categoryType)
                .ConfigureAwait(false);
            var result = mapper.Map<CategoryDto>(category);
            return result;
        }
        public async Task<IEnumerable<CategoryDto>> GetCategoryList()
        {
            var categoryList = await categoryRepository.ListAsync().ConfigureAwait(false);
            var categoryModels = mapper.Map<IEnumerable<CategoryDto>>(categoryList);
            return categoryModels;
        }
    }
}

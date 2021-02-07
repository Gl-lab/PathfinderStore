using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.DTO;
using Pathfinder.Core.Repositories;
using Pathfinder.Utils.Paging;
using AutoMapper;

namespace Pathfinder.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
       // private readonly IAppLogger<CategoryService> logger;
        private readonly IMapper mapper;

        public CategoryService(ICategoryRepository categoryRepository/*, IAppLogger<CategoryService> logger*/, IMapper mapper)
        {
            this.categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
         //   this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CategoryDto> GetById(int id)
        {
            var category = await categoryRepository
                .GetByIdAsync(id)
                .ConfigureAwait(false);
            var result = mapper.Map<CategoryDto>(category);
            return result;
        }
        public async Task<IEnumerable<CategoryDto>> GetCategoryList()
        {
            var categoryList = await categoryRepository.ListAllAsync().ConfigureAwait(false);
            var categoryModels = mapper.Map<IEnumerable<CategoryDto>>(categoryList);
            return categoryModels;
        }

        public async Task<IPagedList<CategoryDto>> SearchCategories(PageSearchArgs args)
        {
            var categoryPagedList = await categoryRepository.SearchAsync(args).ConfigureAwait(false);
            var categoryModels = mapper.Map<List<CategoryDto>>(categoryPagedList.Items);
            var categoryModelPagedList = new PagedList<CategoryDto>(
                categoryPagedList.PageIndex,
                categoryPagedList.PageSize,
                categoryPagedList.TotalCount,
                categoryPagedList.TotalPages,
                categoryModels);

            return categoryModelPagedList;
        }
    }
}

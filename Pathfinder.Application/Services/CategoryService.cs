using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Mapper;
using Pathfinder.Application.Models;
using Pathfinder.Core.Entities;
using Pathfinder.Core.Interfaces;
using Pathfinder.Core.Paging;
using Pathfinder.Core.Repositories;
using Pathfinder.Core.Repositories.Base;
using Pathfinder.Infrastructure.Paging;
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

        public async Task<CategoryModel> GetById(int id)
        {
            var category = await categoryRepository.GetByIdAsync(id).ConfigureAwait(false);
            var result = mapper.Map<CategoryModel>(category);
            return result;
        }
        public async Task<IEnumerable<CategoryModel>> GetCategoryList()
        {
            var categoryList = await categoryRepository.ListAllAsync().ConfigureAwait(false);
            var categoryModels = mapper.Map<IEnumerable<CategoryModel>>(categoryList);
            return categoryModels;
        }

        public async Task<IPagedList<CategoryModel>> SearchCategories(PageSearchArgs args)
        {
            var categoryPagedList = await categoryRepository.SearchCategoriesAsync(args).ConfigureAwait(false);
            var categoryModels = mapper.Map<List<CategoryModel>>(categoryPagedList.Items);
            var categoryModelPagedList = new PagedList<CategoryModel>(
                categoryPagedList.PageIndex,
                categoryPagedList.PageSize,
                categoryPagedList.TotalCount,
                categoryPagedList.TotalPages,
                categoryModels);

            return categoryModelPagedList;
        }
    }
}

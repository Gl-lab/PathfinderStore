using Pathfinder.Core.Entities;
using Pathfinder.Core.Paging;
using Pathfinder.Core.Repositories;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Infrastructure.Paging;
using Pathfinder.Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pathfinder.Infrastructure.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(PgDbContext context)
            : base(context)
        {
        }

        public override async Task<Product> GetByIdAsync(int id)
        {
            //TODO: should be refactored
            var products = await GetAsync(p => p.Id == id, null, new List<Expression<Func<Product, object>>> { p => p.Category });
            return products.FirstOrDefault();
        }

        public async Task<IEnumerable<Product>> GetProductListAsync()
        {
            return await ListAllAsync();
        }

        public Task<IPagedList<Product>> SearchProductsAsync(PageSearchArgs args)
        {
            var query = Table.Include(p => p.Category);

            var orderByList = new List<Tuple<SortingOption, Expression<Func<Product, object>>>>();

            if (args.SortingOptions != null)
            {
                foreach (var sortingOption in args.SortingOptions)
                {
                    switch (sortingOption.Field)
                    {
                        case "id":
                            orderByList.Add(new Tuple<SortingOption, Expression<Func<Product, object>>>(sortingOption, p => p.Id));
                            break;
                        case "name":
                            orderByList.Add(new Tuple<SortingOption, Expression<Func<Product, object>>>(sortingOption, p => p.Name));
                            break;
                        case "price":
                            orderByList.Add(new Tuple<SortingOption, Expression<Func<Product, object>>>(sortingOption, p => p.Price));
                            break;
                        case "category.name":
                            orderByList.Add(new Tuple<SortingOption, Expression<Func<Product, object>>>(sortingOption, p => p.Category.Name));
                            break;
                    }
                }
            }

            if (orderByList.Count == 0)
            {
                orderByList.Add(new Tuple<SortingOption, Expression<Func<Product, object>>>(new SortingOption { Direction = SortingOption.SortingDirection.ASC }, p => p.Id));
            }

            var filterList = new List<Tuple<FilteringOption, Expression<Func<Product, bool>>>>();

            if (args.FilteringOptions != null)
            {
                foreach (var filteringOption in args.FilteringOptions)
                {
                    switch (filteringOption.Field)
                    {
                        case "id":
                            filterList.Add(new Tuple<FilteringOption, Expression<Func<Product, bool>>>(filteringOption, p => p.Id == (int)filteringOption.Value));
                            break;
                        case "name":
                            filterList.Add(new Tuple<FilteringOption, Expression<Func<Product, bool>>>(filteringOption, p => p.Name.Contains((string)filteringOption.Value)));
                            break;
                        case "price":
                            filterList.Add(new Tuple<FilteringOption, Expression<Func<Product, bool>>>(filteringOption, p => p.Price == (int)filteringOption.Value));
                            break;
                        case "category.name":
                            filterList.Add(new Tuple<FilteringOption, Expression<Func<Product, bool>>>(filteringOption, p => p.Category.Name.Contains((string)filteringOption.Value)));
                            break;
                    }
                }
            }

            var productPagedList = new PagedList<Product>(query, new PagingArgs { PageIndex = args.PageIndex, PageSize = args.PageSize, PagingStrategy = args.PagingStrategy }, orderByList, filterList);

            return Task.FromResult<IPagedList<Product>>(productPagedList);
        }

        public async Task<IEnumerable<Product>> GetProductByNameAsync(string productName)
        {
            return await TableNoTracking.Where(p => p.Name.ToLower().Contains(productName.ToLower()))
                                        .Include(p => p.Category)
                                        .ToListAsync();
        }

        public async Task<Product> GetProductByIdWithCategoryAsync(int productId)
        {
            return await GetByIdAsync(productId);
        }

        public async Task<IEnumerable<Product>> GetProductByCategoryAsync(int categoryId)
        {
            return await TableNoTracking
                .Where(x => x.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}

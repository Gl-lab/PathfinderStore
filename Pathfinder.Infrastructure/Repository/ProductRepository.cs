﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Repositories;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Infrastructure.Repository.Base;
using Pathfinder.Utils.Paging;

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
            return await TableNoTracking.Include(e => e.Category).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Product>> GetListAsync()
        {
            return await ListAsync()
                .ConfigureAwait(false);
        }

        public Task<IPagedList<Product>> SearchAsync(PageSearchArgs args)
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
                            orderByList.Add(
                                new Tuple<SortingOption, Expression<Func<Product, object>>>(sortingOption, p => p.Id));
                            break;
                        case "name":
                            orderByList.Add(
                                new Tuple<SortingOption, Expression<Func<Product, object>>>(sortingOption,
                                    p => p.Name));
                            break;
                        case "price":
                            orderByList.Add(
                                new Tuple<SortingOption, Expression<Func<Product, object>>>(sortingOption,
                                    p => p.Price));
                            break;
                        case "category.name":
                            orderByList.Add(
                                new Tuple<SortingOption, Expression<Func<Product, object>>>(sortingOption,
                                    p => p.Category.Name));
                            break;
                    }
                }
            }

            if (orderByList.Count == 0)
            {
                orderByList.Add(new Tuple<SortingOption, Expression<Func<Product, object>>>(
                    new SortingOption { Direction = SortingOption.SortingDirection.ASC }, p => p.Id));
            }

            var filterList = new List<Tuple<FilteringOption, Expression<Func<Product, bool>>>>();

            if (args.FilteringOptions != null)
            {
                foreach (var filteringOption in args.FilteringOptions)
                {
                    switch (filteringOption.Field)
                    {
                        case "id":
                            filterList.Add(new Tuple<FilteringOption, Expression<Func<Product, bool>>>(filteringOption,
                                p => p.Id == (int)filteringOption.Value));
                            break;
                        case "name":
                            filterList.Add(new Tuple<FilteringOption, Expression<Func<Product, bool>>>(filteringOption,
                                p => p.Name.Contains((string)filteringOption.Value)));
                            break;
                        case "price":
                            filterList.Add(new Tuple<FilteringOption, Expression<Func<Product, bool>>>(filteringOption,
                                p => p.Price == (int)filteringOption.Value));
                            break;
                        case "category.name":
                            filterList.Add(new Tuple<FilteringOption, Expression<Func<Product, bool>>>(filteringOption,
                                p => p.Category.Name.Contains((string)filteringOption.Value)));
                            break;
                    }
                }
            }

            var productPagedList = new PagedList<Product>(query,
                new PagingArgs
                    { PageIndex = args.PageIndex, PageSize = args.PageSize, PagingStrategy = args.PagingStrategy },
                orderByList, filterList);

            return Task.FromResult<IPagedList<Product>>(productPagedList);
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string productName)
        {
            return await TableNoTracking.Where(p => p.Name.Contains(productName, StringComparison.OrdinalIgnoreCase))
                .Include(p => p.Category)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<Product> GetByName(string name)
        {
            return await Table.FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<Product> GetByIdWithCategoryAsync(int productId)
        {
            return await GetByIdAsync(productId)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Product>> GetListByCategoryAsync(CategoryType categoryType)
        {
            return await TableNoTracking
                .Where(x => x.CategoryType == categoryType)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
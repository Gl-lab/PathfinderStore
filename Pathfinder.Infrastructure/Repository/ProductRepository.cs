using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Repositories;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Infrastructure.Repository.Base;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Infrastructure.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(PathfinderDbContext context)
            : base(context)
        {
        }

        public override async Task<Product> GetByIdAsync(int id)
        {
            return await TableNoTracking
                //.Include(e => e.Category)
               .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Product>> GetListAsync()
        {
            return Table.ToList();
        }

        public Task<List<Product>> SearchAsync(PageSearchArgs args)
        {
            throw new NotImplementedException();
            // IIncludableQueryable<Product, Category> query = Table;
            //
            // List<Tuple<SortingOption, Expression<Func<Product, object>>>> orderByList = new List<Tuple<SortingOption, Expression<Func<Product, object>>>>();
            //
            // if (args.SortingOptions != null)
            // {
            //     foreach (SortingOption sortingOption in args.SortingOptions)
            //     {
            //         switch (sortingOption.Field)
            //         {
            //             case "id":
            //                 orderByList.Add(
            //                     new Tuple<SortingOption, Expression<Func<Product, object>>>(sortingOption, p => p.Id));
            //                 break;
            //             case "name":
            //                 orderByList.Add(
            //                     new Tuple<SortingOption, Expression<Func<Product, object>>>(sortingOption,
            //                         p => p.Name));
            //                 break;
            //             case "price":
            //                 orderByList.Add(
            //                     new Tuple<SortingOption, Expression<Func<Product, object>>>(sortingOption,
            //                         p => p.Price));
            //                 break;
            //             case "category.name":
            //                 orderByList.Add(
            //                     new Tuple<SortingOption, Expression<Func<Product, object>>>(sortingOption,
            //                         p => p.Category.Name));
            //                 break;
            //         }
            //     }
            // }
            //
            // if (orderByList.Count == 0)
            // {
            //     orderByList.Add(new Tuple<SortingOption, Expression<Func<Product, object>>>(
            //         new SortingOption { Direction = SortingOption.SortingDirection.ASC }, p => p.Id));
            // }
            //
            // List<Tuple<FilteringOption, Expression<Func<Product, bool>>>> filterList = new List<Tuple<FilteringOption, Expression<Func<Product, bool>>>>();
            //
            // if (args.FilteringOptions != null)
            // {
            //     foreach (FilteringOption filteringOption in args.FilteringOptions)
            //     {
            //         switch (filteringOption.Field)
            //         {
            //             case "id":
            //                 filterList.Add(new Tuple<FilteringOption, Expression<Func<Product, bool>>>(filteringOption,
            //                     p => p.Id == (int)filteringOption.Value));
            //                 break;
            //             case "name":
            //                 filterList.Add(new Tuple<FilteringOption, Expression<Func<Product, bool>>>(filteringOption,
            //                     p => p.Name.Contains((string)filteringOption.Value)));
            //                 break;
            //             case "price":
            //                 filterList.Add(new Tuple<FilteringOption, Expression<Func<Product, bool>>>(filteringOption,
            //                     p => p.Price == (int)filteringOption.Value));
            //                 break;
            //             case "category.name":
            //                 filterList.Add(new Tuple<FilteringOption, Expression<Func<Product, bool>>>(filteringOption,
            //                     p => p.Category.Name.Contains((string)filteringOption.Value)));
            //                 break;
            //         }
            //     }
            // }
            //
            // PagedList<Product> productPagedList = new PagedList<Product>(query,
            //     new PagingArgs
            //         { PageIndex = args.PageIndex, PageSize = args.PageSize, PagingStrategy = args.PagingStrategy },
            //     orderByList, filterList);
            //
            // return Task.FromResult<IPagedList<Product>>(productPagedList);
        }

        public async Task<List<Product>> SearchByNameAsync(string productName)
        {
            throw new NotImplementedException();
            // return await TableNoTracking.Where(p => p.Name.Contains(productName, StringComparison.OrdinalIgnoreCase))
            //     .Include(p => p.Category)
            //     .ToListAsync().ConfigureAwait(false);
        }

        public async Task<Product> GetByName(string name)
        {
            throw new NotImplementedException();
            //return await Table.FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<Product> GetByIdWithCategoryAsync(int productId)
        {
            return await GetByIdAsync(productId)
                .ConfigureAwait(false);
        }

        public async Task<List<Product>> GetListByCategoryAsync(CategoryType categoryType)
        {
            return await TableNoTracking
                .Where(x => x.CategoryType == categoryType)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
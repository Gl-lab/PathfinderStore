using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.Repositories;
using Pathfinder.Infrastructure.Data;

namespace Pathfinder.Infrastructure.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PgDbContext dbContext;
        public CategoryRepository(PgDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ICollection<Category>> ListAsync()
        {
            return await dbContext
                .CategoryList
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<Category> GetAsync(CategoryType categoryType)
        {
            return await dbContext
                .CategoryList
                .FirstAsync(e => e.CategoryType == categoryType)
                .ConfigureAwait(false);
        }
    }
}

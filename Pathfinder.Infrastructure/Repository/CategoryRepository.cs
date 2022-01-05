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
        private readonly PgDbContext _dbContext;

        public CategoryRepository(PgDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ICollection<Category>> ListAsync()
        {
            return await _dbContext
                .Category
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<Category> GetAsync(CategoryType categoryType)
        {
            return await _dbContext
                .Category
                .FirstAsync(e => e.CategoryType == categoryType)
                .ConfigureAwait(false);
        }
    }
}
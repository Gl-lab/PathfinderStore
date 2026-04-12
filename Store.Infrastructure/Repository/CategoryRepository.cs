using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Store.Application.Repositories;
using Pathfinder.Store.Infrastructure.Data;

namespace Pathfinder.Store.Infrastructure.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly StoreDbContext _dbContext;

    public CategoryRepository( StoreDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<ICollection<Category>> ListAsync()
    {
        return await _dbContext
                    .Category
                    .ToListAsync()
                    .ConfigureAwait( false );
    }

    public async Task<Category> GetAsync( CategoryType categoryType )
    {
        return await _dbContext
                    .Category
                    .FirstAsync( e => e.CategoryType == categoryType )
                    .ConfigureAwait( false );
    }
}
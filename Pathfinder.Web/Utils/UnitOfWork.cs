using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.Web.Utils;

public class UnitOfWork: IUnitOfWork
{
    private readonly List<DbContext> _dbContexts = new();

    public void AddDbContext( DbContext? dbContext )
    {
        _dbContexts.Add( dbContext ?? throw new ArgumentNullException( nameof( dbContext ) ) );
    }
    public async Task Commit()
    {
        foreach ( DbContext dbContext in _dbContexts )
        {
            await dbContext.SaveChangesAsync();
        }
    }

    public Task Rollback()
    {
        throw new System.NotImplementedException();
    }

    public Task BeginTransaction()
    {
        throw new System.NotImplementedException();
    }
}
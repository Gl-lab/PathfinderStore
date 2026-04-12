using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.Utils.UnitOfWork;

namespace CharacterManagement.Infrastructure.Tests.TestSupport;

public sealed class TestUnitOfWork : IUnitOfWork
{
    private readonly CharacterManagementDbContext _dbContext;

    public TestUnitOfWork( CharacterManagementDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public Task Commit()
    {
        return _dbContext.SaveChangesAsync();
    }

    public Task Rollback()
    {
        return Task.CompletedTask;
    }

    public Task BeginTransaction()
    {
        return Task.CompletedTask;
    }
}

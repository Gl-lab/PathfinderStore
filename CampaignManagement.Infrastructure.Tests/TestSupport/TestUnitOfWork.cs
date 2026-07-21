using Pathfinder.CampaignManagement.Infrastructure.Data;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CampaignManagement.Infrastructure.Tests.TestSupport;

internal sealed class TestUnitOfWork : IUnitOfWork
{
    private readonly CampaignManagementDbContext _dbContext;

    public TestUnitOfWork( CampaignManagementDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task Commit()
    {
        await _dbContext.SaveChangesAsync();
    }

    public Task Rollback() => throw new NotSupportedException();

    public Task BeginTransaction() => throw new NotSupportedException();
}

using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Infrastructure.Data;

namespace CharacterManagement.Infrastructure.Tests.TestSupport;

public static class TestCharacterManagementDbContextFactory
{
    public static CharacterManagementDbContext Create()
    {
        DbContextOptions<CharacterManagementDbContext> options = new DbContextOptionsBuilder<CharacterManagementDbContext>()
            .UseInMemoryDatabase( Guid.NewGuid().ToString( "N" ) )
            .Options;

        CharacterManagementDbContext dbContext = new CharacterManagementDbContext( options );
        dbContext.Database.EnsureCreated();

        return dbContext;
    }
}

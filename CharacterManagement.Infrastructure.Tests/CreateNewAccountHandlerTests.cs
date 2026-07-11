using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Pathfinder.CharacterManagement.Application.UseCases.Accounts;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;
using CharacterManagement.Infrastructure.Tests.TestSupport;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class CreateNewAccountHandlerTests
{
    [Fact]
    public async Task EnsureCreated_SeedsAccountsForDefaultUsers()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();

        List<Account> accounts = await dbContext.Account
            .AsNoTracking()
            .OrderBy( account => account.UserId )
            .ToListAsync();

        Assert.Collection(
            accounts,
            adminAccount =>
            {
                Assert.Equal( 1, adminAccount.Id );
                Assert.Equal( 1, adminAccount.UserId );
                Assert.Equal( "System", adminAccount.Name );
                Assert.Equal( "Administrator", adminAccount.Surname );
            },
            memberAccount =>
            {
                Assert.Equal( 2, memberAccount.Id );
                Assert.Equal( 2, memberAccount.UserId );
                Assert.Equal( "Test", memberAccount.Name );
                Assert.Equal( "User", memberAccount.Surname );
            } );
    }

    [Fact]
    public async Task Handle_NewUser_PersistsAccountInDbContext()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        EnsureAccountExistsHandler handler = CreateHandler( dbContext );
        EnsureAccountExistsCommand command = new EnsureAccountExistsCommand( 42, "Alrik", "Stone" );

        await handler.Handle( command, CancellationToken.None );

        Account? savedAccount = await dbContext.Account
            .AsNoTracking()
            .SingleOrDefaultAsync( account => account.UserId == command.UserId );

        Assert.NotNull( savedAccount );
        Assert.Equal( command.UserId, savedAccount.UserId );
        Assert.Equal( command.Name, savedAccount.Name );
        Assert.Equal( command.Surname, savedAccount.Surname );
    }

    [Fact]
    public async Task Handle_DuplicateUserId_DoesNotCreateDuplicate()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account existingAccount = new Account
        {
            UserId = 77,
            Name = "Existing",
            Surname = "User",
        };

        dbContext.Account.Add( existingAccount );
        await dbContext.SaveChangesAsync();

        EnsureAccountExistsHandler handler = CreateHandler( dbContext );
        EnsureAccountExistsCommand command = new EnsureAccountExistsCommand( 77, "Duplicate", "User" );

        await handler.Handle( command, CancellationToken.None );

        List<Account> accounts = await dbContext.Account
            .AsNoTracking()
            .Where( account => account.UserId == command.UserId )
            .ToListAsync();

        Account savedAccount = Assert.Single( accounts );
        Assert.Equal( existingAccount.Name, savedAccount.Name );
        Assert.Equal( existingAccount.Surname, savedAccount.Surname );
    }

    [Fact]
    public async Task Handle_RepeatedRun_IsSafeAndKeepsSingleAccount()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        EnsureAccountExistsHandler handler = CreateHandler( dbContext );
        EnsureAccountExistsCommand command = new EnsureAccountExistsCommand( 91, "Meri", "Gold" );

        await handler.Handle( command, CancellationToken.None );
        await handler.Handle( command, CancellationToken.None );

        List<Account> accounts = await dbContext.Account
            .AsNoTracking()
            .Where( account => account.UserId == command.UserId )
            .ToListAsync();

        Account savedAccount = Assert.Single( accounts );
        Assert.Equal( command.Name, savedAccount.Name );
        Assert.Equal( command.Surname, savedAccount.Surname );
    }

    private static EnsureAccountExistsHandler CreateHandler( CharacterManagementDbContext dbContext )
    {
        AccountRepository accountRepository = new AccountRepository( dbContext );
        TestUnitOfWork unitOfWork = new TestUnitOfWork( dbContext );

        return new EnsureAccountExistsHandler( accountRepository, unitOfWork, NullLogger<EnsureAccountExistsHandler>.Instance );
    }
}

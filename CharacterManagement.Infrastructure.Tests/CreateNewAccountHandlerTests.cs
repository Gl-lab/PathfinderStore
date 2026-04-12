using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.UseCases.Accounts;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;
using CharacterManagement.Infrastructure.Tests.TestSupport;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class CreateNewAccountHandlerTests
{
    [Fact]
    public async Task Handle_NewUser_PersistsAccountInDbContext()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        CreateNewAccountHandler handler = CreateHandler( dbContext );
        CreateNewAccountCommand command = new CreateNewAccountCommand( 42, "Alrik", "Stone" );

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
    public async Task Handle_DuplicateUserId_ThrowsAndDoesNotCreateDuplicate()
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

        CreateNewAccountHandler handler = CreateHandler( dbContext );
        CreateNewAccountCommand command = new CreateNewAccountCommand( 77, "Duplicate", "User" );

        await Assert.ThrowsAsync<CharacterManagementException>( async () =>
            await handler.Handle( command, CancellationToken.None ) );

        List<Account> accounts = await dbContext.Account
            .AsNoTracking()
            .Where( account => account.UserId == command.UserId )
            .ToListAsync();

        Account savedAccount = Assert.Single( accounts );
        Assert.Equal( existingAccount.Name, savedAccount.Name );
        Assert.Equal( existingAccount.Surname, savedAccount.Surname );
    }

    private static CreateNewAccountHandler CreateHandler( CharacterManagementDbContext dbContext )
    {
        AccountRepository accountRepository = new AccountRepository( dbContext );
        TestUnitOfWork unitOfWork = new TestUnitOfWork( dbContext );

        return new CreateNewAccountHandler( accountRepository, unitOfWork );
    }
}

using Microsoft.EntityFrameworkCore;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.UseCases.Characters;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Infrastructure.Data;
using Pathfinder.CharacterManagement.Infrastructure.Repositories;
using CharacterManagement.Infrastructure.Tests.TestSupport;

namespace CharacterManagement.Infrastructure.Tests;

public sealed class DeleteCharacterHandlerTests
{
    [Fact]
    public async Task Handle_ExistingCharacterForCurrentUser_DeletesCharacter()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 110 );
        DraftCharacter draftCharacter = await CreateCharacterAsync( dbContext, account.Id, "Delete Me" );
        DeleteCharacterHandler handler = CreateHandler( dbContext );
        DeleteCharacterCommand command = new DeleteCharacterCommand( account.UserId, draftCharacter.Id );

        await handler.Handle( command, CancellationToken.None );

        DraftCharacter? deletedCharacter = await dbContext.Character
            .AsNoTracking()
            .SingleOrDefaultAsync( entity => entity.Id == draftCharacter.Id );

        Assert.Null( deletedCharacter );
    }

    [Fact]
    public async Task Handle_CharacterBelongsToAnotherUser_ThrowsAndDoesNotDeleteCharacter()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account ownerAccount = await CreateAccountAsync( dbContext, 210 );
        DraftCharacter draftCharacter = await CreateCharacterAsync( dbContext, ownerAccount.Id, "Protected Character" );
        DeleteCharacterHandler handler = CreateHandler( dbContext );
        DeleteCharacterCommand command = new DeleteCharacterCommand( 999, draftCharacter.Id );

        await Assert.ThrowsAsync<CharacterManagementException>( async () =>
            await handler.Handle( command, CancellationToken.None ) );

        int characterCount = await dbContext.Character.CountAsync();
        Assert.Equal( 1, characterCount );
    }

    [Fact]
    public async Task Handle_CharacterDoesNotExist_Throws()
    {
        await using CharacterManagementDbContext dbContext = TestCharacterManagementDbContextFactory.Create();
        Account account = await CreateAccountAsync( dbContext, 310 );
        DeleteCharacterHandler handler = CreateHandler( dbContext );
        DeleteCharacterCommand command = new DeleteCharacterCommand( account.UserId, 9999 );

        await Assert.ThrowsAsync<CharacterManagementException>( async () =>
            await handler.Handle( command, CancellationToken.None ) );
    }

    private static DeleteCharacterHandler CreateHandler( CharacterManagementDbContext dbContext )
    {
        CharacterRepository characterRepository = new CharacterRepository( dbContext );
        TestUnitOfWork unitOfWork = new TestUnitOfWork( dbContext );

        return new DeleteCharacterHandler( characterRepository, unitOfWork );
    }

    private static async Task<Account> CreateAccountAsync( CharacterManagementDbContext dbContext, int userId )
    {
        Account account = new Account
        {
            UserId = userId,
            Name = "Name",
            Surname = "Surname",
        };

        dbContext.Account.Add( account );
        await dbContext.SaveChangesAsync();

        return account;
    }

    private static async Task<DraftCharacter> CreateCharacterAsync(
        CharacterManagementDbContext dbContext,
        int accountId,
        string name )
    {
        DraftCharacter draftCharacter = DraftCharacter.Create( accountId, name, AncestryType.Human );

        dbContext.Character.Add( draftCharacter );
        await dbContext.SaveChangesAsync();

        return draftCharacter;
    }
}

using MediatR;
using Pathfinder.Application.Services;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class DeleteCharacterHandler : IRequestHandler<DeleteCharacterCommand>
{
    private readonly IAccountService _accountService;

    public DeleteCharacterHandler( IAccountService accountService )
    {
        _accountService = accountService;
    }

    public async Task Handle( DeleteCharacterCommand request, CancellationToken cancellationToken )
    {
        await _accountService.DeleteCharacterAsync( request.DeletedCharacterId );
    }
}
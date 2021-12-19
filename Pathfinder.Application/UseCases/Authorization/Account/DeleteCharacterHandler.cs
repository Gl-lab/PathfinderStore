using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.Interfaces;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class DeleteCharacterHandler: IRequestHandler<DeleteCharacterCommand>
{
    private readonly IAccountService _accountService;

    public DeleteCharacterHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<Unit> Handle(DeleteCharacterCommand request, CancellationToken cancellationToken)
    {
        await _accountService.DeleteCharacterAsync(request.DeletedCharacterId);
        return Unit.Value;
    }
}
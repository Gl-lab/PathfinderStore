using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.Interfaces;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class SetCurrentCharacterHandler: IRequestHandler<SetCurrentCharacterCommand>
{
    private readonly IAccountService _accountService;

    public SetCurrentCharacterHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<Unit> Handle(SetCurrentCharacterCommand request, CancellationToken cancellationToken)
    {
        await _accountService.SetCurrentCharacterAsync(request.CharacterId);
        return Unit.Value;
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.Interfaces;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class CreateCharacterHandler: IRequestHandler<CreateCharacterCommand>
{
    private readonly IAccountService _accountService;

    public CreateCharacterHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<Unit> Handle(CreateCharacterCommand request, CancellationToken cancellationToken)
    {
        await _accountService.CreateCharacterAsync(request.Character);
        return Unit.Value;
    }
}
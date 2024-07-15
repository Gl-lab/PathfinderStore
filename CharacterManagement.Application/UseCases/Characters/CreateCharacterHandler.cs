using MediatR;
using Pathfinder.Application.Services;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class CreateCharacterHandler: IRequestHandler<CreateCharacterCommand>
{
    private readonly IAccountService _accountService;

    public CreateCharacterHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task Handle(CreateCharacterCommand request, CancellationToken cancellationToken)
    {
        //await _accountService.CreateCharacterAsync(request.Character);
    }
}
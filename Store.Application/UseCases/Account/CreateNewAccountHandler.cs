using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.Services;

namespace Pathfinder.Application.UseCases.Account;

public class CreateNewAccountHandler: IRequestHandler<CreateNewAccountCommand>
{
    private readonly IAccountService _accountService;

    public CreateNewAccountHandler( IAccountService accountService )
    {
        _accountService = accountService;
    }

    public async Task Handle( CreateNewAccountCommand request, CancellationToken cancellationToken )
    {
        await _accountService.CreateAsync( request.UserId );
    }
}
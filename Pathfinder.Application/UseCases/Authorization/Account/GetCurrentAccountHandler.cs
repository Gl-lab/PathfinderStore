using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Interfaces;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class GetCurrentAccountHandler: IRequestHandler<GetCurrentAccountCommand, AccountDto>
{
    private readonly IAccountService _accountService;

    public GetCurrentAccountHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<AccountDto> Handle(GetCurrentAccountCommand request, CancellationToken cancellationToken)
    {
        return await _accountService.GetCurrentAccountAsync();
    }
}
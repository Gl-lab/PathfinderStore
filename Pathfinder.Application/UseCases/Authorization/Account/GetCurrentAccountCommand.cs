using MediatR;
using Pathfinder.Application.DTO;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class GetCurrentAccountCommand: IRequest<AccountDto>
{
}
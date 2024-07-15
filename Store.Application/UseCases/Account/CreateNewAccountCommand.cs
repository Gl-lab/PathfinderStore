using MediatR;

namespace Pathfinder.Application.UseCases.Account;

public class CreateNewAccountCommand : IRequest
{
    public CreateNewAccountCommand( int userId )
    {
        UserId = userId;
    }

    public int UserId { get; init;}
}
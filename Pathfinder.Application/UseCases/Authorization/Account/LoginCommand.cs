using MediatR;
using Pathfinder.Application.DTO.Authentication.Account;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class LoginCommand : IRequest<LoginOutput>
{
    public LoginCommand(string userNameOrEmail, string password)
    {
        UserNameOrEmail = userNameOrEmail;
        Password = password;
    }

    public string UserNameOrEmail { get; }
    public string Password { get; }
}
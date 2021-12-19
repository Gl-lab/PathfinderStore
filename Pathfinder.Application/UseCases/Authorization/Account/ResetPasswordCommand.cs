using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class ResetPasswordCommand : IRequest<IdentityResult>
{
    public ResetPasswordCommand(string userNameOrEmail, string password, string token)
    {
        UserNameOrEmail = userNameOrEmail;
        Password = password;
        Token = token;
    }

    public string UserNameOrEmail { get; }
    public string Password { get; }
    public string Token { get; }
}
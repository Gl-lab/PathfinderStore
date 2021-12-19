using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class RegisterUserCommand: IRequest<IdentityResult>
{
    public RegisterUserCommand(string userName, string email, string password)
    {
        UserName = userName;
        Email = email;
        Password = password;
    }

    public string UserName { get; }
    public string Email { get; }
    public string Password { get; }
}
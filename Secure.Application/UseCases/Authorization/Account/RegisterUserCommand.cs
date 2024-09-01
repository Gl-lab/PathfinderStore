using MediatR;
using Pathfinder.Secure.Application.DTO.Authentication.Account;

namespace Pathfinder.Secure.Application.UseCases.Authorization.Account;

public class RegisterUserCommand : IRequest<RegisterUserOutput>
{
    public RegisterUserCommand( string userName,
                                string email,
                                string password )
    {
        UserName = userName;
        Email = email;
        Password = password;
    }

    public string UserName { get; }
    public string Email { get; }
    public string Password { get; }
}
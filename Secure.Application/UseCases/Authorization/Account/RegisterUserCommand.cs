using MediatR;
using Secure.Application.DTO.Authentication.Account;

namespace Secure.Application.UseCases.Authorization.Account;

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
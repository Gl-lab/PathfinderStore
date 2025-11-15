using MediatR;
using Pathfinder.Secure.Application.DTO.Authentication.Account;

namespace Pathfinder.Secure.Application.UseCases.Authorization.Account;

public class RegisterUserCommand : IRequest<RegisterUserOutput>
{
    public RegisterUserCommand(
        string userName,
        string email,
        string password,
        string? name = null,
        string? surname = null )
    {
        UserName = userName;
        Email = email;
        Password = password;
        Name = name;
        Surname = surname;
    }

    public string UserName { get; }
    public string Email { get; }
    public string Password { get; }
    public string? Name { get; }
    public string? Surname { get; }
}
using Microsoft.AspNetCore.Identity;

namespace Pathfinder.Secure.Application.DTO.Authentication.Account;

public class RegisterUserOutput
{
    public RegisterUserOutput( IdentityResult identityResult, int? userId )
    {
        IdentityResult = identityResult;
        UserId = userId;
    }

    public IdentityResult IdentityResult { get; init; }
    public int? UserId { get; init; }
}
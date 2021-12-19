using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class ChangePasswordCommand : IRequest<IdentityResult>
{
    public ChangePasswordCommand(string userName, string currentPassword, string newPassword, string passwordRepeat)
    {
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
        PasswordRepeat = passwordRepeat;
        UserName = userName;
    }

    public string UserName { get; }
    public string CurrentPassword { get; }

    public string NewPassword { get; }

    public string PasswordRepeat { get; }
}
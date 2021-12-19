using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Interfaces.Auth;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, IdentityResult>
{
    private readonly IUserService _userService;

    public ChangePasswordHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IdentityResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.PasswordRepeat)
        {
            return IdentityResult.Failed(new List<IdentityError>
            {
                new()
                {
                    Code = "PasswordsDoesNotMatch",
                    Description = "Passwords doesn't match"
                }
            }.ToArray());
        }

        var user = await _userService.FindByNameAsync(request.UserName).ConfigureAwait(false);
        return await _userService.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword)
            .ConfigureAwait(false);
    }
}
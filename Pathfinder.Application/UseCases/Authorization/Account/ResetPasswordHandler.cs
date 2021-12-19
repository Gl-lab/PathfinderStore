using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Interfaces.Auth;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class ResetPasswordHandler: IRequestHandler<ResetPasswordCommand, IdentityResult>
{
    private readonly IUserService _userService;

    public ResetPasswordHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IdentityResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByNameAsync(request.UserNameOrEmail) ?? await _userService.FindByEmailAsync(request.UserNameOrEmail);

        if (user == null)
        {
            return IdentityResult.Failed(new List<IdentityError>
            {
                new()
                {
                    Code = "UserNotFound",
                    Description = "User is not found"
                }
            }.ToArray());
        }

        return await _userService.ResetPasswordAsync(user, request.Token, request.Password);
    }
}
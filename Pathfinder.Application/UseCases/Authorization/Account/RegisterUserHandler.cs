using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Core.UnitOfWork;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserHandler(IUserService userService, IUnitOfWork unitOfWork)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
    }


    public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByEmailAsync(request.Email).ConfigureAwait(false);
        if (user != null)
        {
            return IdentityResult.Failed(new List<IdentityError>
            {
                new()
                {
                    Code = "EmailAlreadyExist",
                    Description = "This email already exists"
                }
            }.ToArray());
        }

        await _unitOfWork.CommitAsync();
        return await _userService.CreateUser(request.UserName, request.Email, request.Password);
    }
}
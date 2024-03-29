﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Core.UnitOfWork;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountService _accountService;

    public RegisterUserHandler(
        IUserService userService,
        IUnitOfWork unitOfWork,
        IAccountService accountService)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _accountService = accountService;
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

        IdentityResult result = await _userService.CreateUser(request.UserName, request.Email, request.Password);
        if (result.Succeeded)
        {
            var createdUser = await _userService.FindByEmailAsync(request.Email);
            await _accountService.CreateAsync(createdUser.Id);
            await _unitOfWork.CommitAsync();
        }

        return result;
    }
}
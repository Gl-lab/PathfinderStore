using Authorization.Authentication.User;
using Authorization.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Utils.UnitOfWork;
using Secure.Application.DTO.Authentication.Account;
using Secure.Application.Services.Authentication;

namespace Secure.Application.UseCases.Authorization.Account;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserOutput>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserHandler(
        IUserService userService,
        IUnitOfWork unitOfWork )
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
    }


    public async Task<RegisterUserOutput> Handle( RegisterUserCommand request, CancellationToken cancellationToken )
    {
        User? user = await _userService.FindByEmailAsync( request.Email ).ConfigureAwait( false );
        if ( user is not null )
        {
            return new RegisterUserOutput( IdentityResult.Failed( new List<IdentityError>
            {
                new()
                {
                    Code = "EmailAlreadyExist",
                    Description = "This email already exists"
                }
            }.ToArray() ), null );
        }

        user = await _userService.FindByNameAsync( request.UserName ).ConfigureAwait( false );
        if ( user is not null )
        {
            return new RegisterUserOutput(IdentityResult.Failed( new List<IdentityError>
            {
                new()
                {
                    Code = "UserNameAlreadyExist",
                    Description = "This user name already exists"
                }
            }.ToArray() ), null);
        }

        IdentityResult result = await _userService.CreateUser( request.UserName, request.Email, request.Password );
        if ( result.Succeeded )
        {
            await _unitOfWork.Commit();
        }
        user = await _userService.FindByNameAsync( request.UserName ).ConfigureAwait( false );
        if ( user is null )
        {
            throw new SecureException( $"User {request.UserName} not created" );
        }
        return new RegisterUserOutput( result, user.Id );

    }
}
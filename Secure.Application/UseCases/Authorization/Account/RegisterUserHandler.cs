using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Contracts.Events;
using Pathfinder.Secure.Application.DTO.Authentication.Account;
using Pathfinder.Secure.Application.Services.Authentication;
using Pathfinder.Secure.Domain.Authentication.User;
using Pathfinder.Secure.Domain.Exceptions;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.Secure.Application.UseCases.Authorization.Account;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserOutput>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBus _bus;

    public RegisterUserHandler(
        IUserService userService,
        IUnitOfWork unitOfWork,
        IBus bus )
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _bus = bus;
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
            return new RegisterUserOutput( IdentityResult.Failed( new List<IdentityError>
            {
                new()
                {
                    Code = "UserNameAlreadyExist",
                    Description = "This user name already exists"
                }
            }.ToArray() ), null );
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

        await _bus.Publish( new UserRegisteredEvent( user.Id ), cancellationToken );
        return new RegisterUserOutput( result, user.Id );
    }
}
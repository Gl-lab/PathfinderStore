using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Pathfinder.Contracts.Events;
using Pathfinder.Secure.Application.DTO.Authentication.Account;
using Pathfinder.Secure.Application.Services.Authentication;
using Pathfinder.Secure.Domain.Authentication.User;
using Pathfinder.Secure.Domain.Exceptions;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.Secure.Application.UseCases.Authorization.Account;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserOutput>
{
    private readonly IBus _bus;
    private readonly ILogger<RegisterUserHandler> _logger;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserHandler(
        IUserService userService,
        IUnitOfWork unitOfWork,
        IBus bus,
        ILogger<RegisterUserHandler> logger )
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _bus = bus;
        _logger = logger;
    }

    public async Task<RegisterUserOutput> Handle( RegisterUserCommand request, CancellationToken cancellationToken )
    {
        User? user = await _userService.FindByEmailAsync( request.Email )
           .ConfigureAwait( false );
        if ( user is not null )
        {
            return new RegisterUserOutput(
                IdentityResult.Failed(
                    new List<IdentityError>
                    {
                        new()
                        {
                            Code = "EmailAlreadyExist",
                            Description = "This email already exists"
                        }
                    }.ToArray() ),
                null );
        }

        user = await _userService.FindByNameAsync( request.UserName )
           .ConfigureAwait( false );
        if ( user is not null )
        {
            return new RegisterUserOutput(
                IdentityResult.Failed(
                    new List<IdentityError>
                    {
                        new()
                        {
                            Code = "UserNameAlreadyExist",
                            Description = "This user name already exists"
                        }
                    }.ToArray() ),
                null );
        }

        IdentityResult result = await _userService.CreateUser( request.UserName, request.Email, request.Password );
        if ( result.Succeeded )
        {
            await _unitOfWork.Commit();

            _logger.LogInformation(
                "User registration committed for user {UserName}.",
                request.UserName );
        }

        user = await _userService.FindByNameAsync( request.UserName )
           .ConfigureAwait( false );
        if ( user is null )
        {
            throw new SecureException( $"User {request.UserName} not created" );
        }

        _logger.LogInformation(
            "Publishing UserRegisteredEvent for user {UserId}.",
            user.Id );

        await _bus.Publish( new UserRegisteredEvent( user.Id, request.Name, request.Surname ), cancellationToken );
        return new RegisterUserOutput( result, user.Id );
    }
}

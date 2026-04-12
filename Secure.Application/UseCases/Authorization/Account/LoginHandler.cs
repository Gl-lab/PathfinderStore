using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.Extensions.Options;
using Pathfinder.Secure.Application.Configuration;
using Pathfinder.Secure.Application.DTO.Authentication.Account;
using Pathfinder.Secure.Application.Services.Authentication;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.Secure.Application.UseCases.Authorization.Account;

public class LoginHandler : IRequestHandler<LoginCommand, LoginOutput>
{
    private readonly IUserService _userService;
    private readonly JwtTokenConfiguration _jwtTokenConfiguration;
    private readonly IUnitOfWork _unitOfWork;

    public LoginHandler( IUserService userService,
                         IOptions<JwtTokenConfiguration> jwtTokenConfiguration,
                         IUnitOfWork unitOfWork )
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _jwtTokenConfiguration = jwtTokenConfiguration.Value;
    }

    public async Task<LoginOutput> Handle( LoginCommand request, CancellationToken cancellationToken )
    {
        ClaimsIdentity? userToVerify =
            await _userService.CreateClaimsIdentityAsync( request.UserNameOrEmail, request.Password );

        if ( userToVerify is null )
        {
            return null!;
        }

        JwtSecurityToken token = new JwtSecurityToken
        (
            issuer: _jwtTokenConfiguration.Issuer,
            audience: _jwtTokenConfiguration.Audience,
            claims: userToVerify.Claims,
            notBefore: _jwtTokenConfiguration.StartDate,
            expires: _jwtTokenConfiguration.EndDate,
            signingCredentials: _jwtTokenConfiguration.SigningCredentials
        );
        await _unitOfWork.Commit();
        return new LoginOutput { Token = new JwtSecurityTokenHandler().WriteToken( token ) };
    }
}
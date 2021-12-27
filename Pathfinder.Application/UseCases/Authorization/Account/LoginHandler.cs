using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.DTO.Authentication.Account;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Application.Services.Authentication;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class LoginHandler : IRequestHandler<LoginCommand, LoginOutput>
{
    private readonly IUserService _userService;
    private readonly JwtTokenConfiguration _jwtTokenConfiguration;

    public LoginHandler(IUserService userService, JwtTokenConfiguration jwtTokenConfiguration)
    {
        _userService = userService;
        _jwtTokenConfiguration = jwtTokenConfiguration;
    }

    public async Task<LoginOutput> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var userToVerify = await _userService.CreateClaimsIdentityAsync(request.UserNameOrEmail, request.Password);
        if (userToVerify == null)
        {
            return null;
        }

        var token = new JwtSecurityToken
        (
            issuer: _jwtTokenConfiguration.Issuer,
            audience: _jwtTokenConfiguration.Audience,
            claims: userToVerify.Claims,
            notBefore: _jwtTokenConfiguration.StartDate,
            expires: _jwtTokenConfiguration.EndDate,
            signingCredentials: _jwtTokenConfiguration.SigningCredentials
        );

        return new LoginOutput { Token = new JwtSecurityTokenHandler().WriteToken(token) };
    }
}
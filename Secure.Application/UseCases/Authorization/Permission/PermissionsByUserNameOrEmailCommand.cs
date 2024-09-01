using MediatR;
using Pathfinder.Secure.Application.DTO.Authentication.Permissions;

namespace Pathfinder.Secure.Application.UseCases.Authorization.Permission;

public class PermissionsByUserNameOrEmailCommand : IRequest<IEnumerable<PermissionDto>>
{
    public PermissionsByUserNameOrEmailCommand(string userNameOrEmail)
    {
        UserNameOrEmail = userNameOrEmail;
    }

    public string UserNameOrEmail { get; }
}
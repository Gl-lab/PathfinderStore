using System.Collections.Generic;
using MediatR;
using Pathfinder.Application.DTO.Authentication.Permissions;

namespace Pathfinder.Application.UseCases.Authorization.Permission
{
    public class PermissionsByUserNameOrEmailCommand : IRequest<IEnumerable<PermissionDto>>
    {
        public PermissionsByUserNameOrEmailCommand(string userNameOrEmail)
        {
            UserNameOrEmail = userNameOrEmail;
        }

        public string UserNameOrEmail { get; }
    }
}
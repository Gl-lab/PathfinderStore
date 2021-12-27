using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.DTO.Authentication.Roles;

namespace Pathfinder.Application.UseCases.Authorization.Roles
{
    public class CreateOrUpdateRoleCommand : IRequest<IdentityResult>
    {
        public RoleDto Role { get; set; } = new();

        public List<int> GrantedPermissionIds { get; set; } = new();
    }
}
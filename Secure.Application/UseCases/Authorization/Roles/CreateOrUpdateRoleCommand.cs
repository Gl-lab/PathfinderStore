using MediatR;
using Microsoft.AspNetCore.Identity;
using Secure.Application.DTO.Authentication.Roles;

namespace Secure.Application.UseCases.Authorization.Roles
{
    public class CreateOrUpdateRoleCommand : IRequest<IdentityResult>
    {
        public RoleDto Role { get; set; } = new();

        public List<int> GrantedPermissionIds { get; set; } = new();
    }
}
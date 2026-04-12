using MediatR;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Secure.Application.DTO.Authentication.Roles;

namespace Pathfinder.Secure.Application.UseCases.Authorization.Roles;

public class CreateOrUpdateRoleCommand : IRequest<IdentityResult>
{
    public RoleDto Role { get; set; } = new();

    public List<int> GrantedPermissionIds { get; set; } = new();
}
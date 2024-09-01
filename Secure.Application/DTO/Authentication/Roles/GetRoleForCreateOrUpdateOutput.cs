using Pathfinder.Secure.Application.DTO.Authentication.Permissions;

namespace Pathfinder.Secure.Application.DTO.Authentication.Roles;

public class GetRoleForCreateOrUpdateOutput
{
    public RoleDto Role { get; set; } = new();

    public List<PermissionDto> AllPermissions { get; set; } = new();

    public List<int> GrantedPermissionIds { get; set; } = new();
}
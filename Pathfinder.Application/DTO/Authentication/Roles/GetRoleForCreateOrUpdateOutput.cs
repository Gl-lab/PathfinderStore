using System.Collections.Generic;
using Pathfinder.Application.DTO.Authentication.Permissions;

namespace Pathfinder.Application.DTO.Authentication.Roles
{
    public class GetRoleForCreateOrUpdateOutput
    {
        public RoleDto Role { get; set; } = new();

        public List<PermissionDto> AllPermissions { get; set; } = new();

        public List<int> GrantedPermissionIds { get; set; } = new();
    }
}
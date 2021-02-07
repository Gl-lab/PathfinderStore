using System;
using System.Collections.Generic;
using Pathfinder.Application.DTO.Auth.Permissions;
using Pathfinder.Application.DTO.Auth.Roles;

namespace Pathfinder.Application.DTO.Auth.Roles
{
    public class GetRoleForCreateOrUpdateOutput
    {
        public RoleDto Role { get; set; } = new();

        public List<PermissionDto> AllPermissions { get; set; } = new();

        public List<int> GrantedPermissionIds { get; set; } = new();
    }
}
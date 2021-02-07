using System;
using System.Collections.Generic;

namespace Pathfinder.Application.DTO.Auth.Roles
{
    public class CreateOrUpdateRoleInput
    {
        public RoleDto Role { get; set; } = new();

        public List<int> GrantedPermissionIds { get; set; } = new();
    }
}

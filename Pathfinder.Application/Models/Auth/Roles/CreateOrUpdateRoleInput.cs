using System;
using System.Collections.Generic;

namespace Pathfinder.Application.Models.Auth.Roles
{
    public class CreateOrUpdateRoleInput
    {
        public RoleModel Role { get; set; } = new RoleModel();

        public List<int> GrantedPermissionIds { get; set; } = new List<int>();
    }
}

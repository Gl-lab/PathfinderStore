using System;
using System.Collections.Generic;
using Pathfinder.Application.Models.Auth.Roles;
using Pathfinder.Application.Models.Auth.Permissions;


namespace Pathfinder.Application.Models.Auth.Roles
{
    public class GetRoleForCreateOrUpdateOutput
    {
        public RoleModel Role { get; set; } = new RoleModel();

        public List<PermissionModel> AllPermissions { get; set; } = new List<PermissionModel>();

        public List<Guid> GrantedPermissionIds { get; set; } = new List<Guid>();
    }
}
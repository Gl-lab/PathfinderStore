using System;
using Pathfinder.Core.Entities.Auth.Permissions;

namespace Pathfinder.Core.Entities.Auth.Roles
{
    public class RolePermission
    {
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        public int PermissionId { get; set; }

        public virtual Permission Permission { get; set; }
    }
}
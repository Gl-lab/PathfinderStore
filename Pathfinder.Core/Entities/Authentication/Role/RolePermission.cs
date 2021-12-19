using Pathfinder.Core.Entities.Authentication.Permissions;

namespace Pathfinder.Core.Entities.Authentication.Role
{
    public class RolePermission
    {
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        public int PermissionId { get; set; }

        public virtual Permission Permission { get; set; }
    }
}
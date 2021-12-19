using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Core.Entities.Authentication.User;

namespace Pathfinder.Core.Entities.Authentication.Role
{
    public class Role : IdentityRole<int>
    {
        public bool IsSystemDefault { get; set; } = false;

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}

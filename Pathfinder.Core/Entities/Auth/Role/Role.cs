using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Core.Entities.Auth.Users;

namespace Pathfinder.Core.Entities.Auth.Roles
{
    public class Role : IdentityRole<int>
    {
        public bool IsSystemDefault { get; set; } = false;

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}

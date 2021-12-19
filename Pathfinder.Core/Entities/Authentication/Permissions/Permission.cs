using System;
using System.Collections.Generic;
using Pathfinder.Core.Entities.Authentication.Role;

namespace Pathfinder.Core.Entities.Authentication.Permissions
{
    public class Permission
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }

        public DateTime ModificationDate { get; set; }

        public int CreatorId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
using System.Collections.Generic;
using System;
using Pathfinder.Core.Entities.Auth.Roles;
using Pathfinder.Core.Entities.Base;

namespace Pathfinder.Core.Entities.Auth.Permissions
{
    public class Permission
    {

        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }

        public DateTime ModificationDate { get; set; }

        public Guid CreatorId { get; set; }
        
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
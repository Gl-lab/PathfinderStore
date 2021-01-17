using System;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Core.Entities.Auth.Roles;

namespace Pathfinder.Core.Entities.Auth.Users
{
    public class UserRole : IdentityUserRole<int>
    {
        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}

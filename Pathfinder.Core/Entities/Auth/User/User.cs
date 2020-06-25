using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Pathfinder.Core.Entities.Auth.Users
{
    public class User : IdentityUser<Guid>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
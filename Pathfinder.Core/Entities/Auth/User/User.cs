using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Pathfinder.Core.Entities.Auth.Users
{
    public class User : IdentityUser<int>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
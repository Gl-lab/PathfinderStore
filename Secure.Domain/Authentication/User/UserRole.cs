using Microsoft.AspNetCore.Identity;

namespace Pathfinder.Secure.Domain.Authentication.User;

public class UserRole : IdentityUserRole<int>
{
    public virtual User User { get; set; }

    public virtual Role.Role Role { get; set; }
}
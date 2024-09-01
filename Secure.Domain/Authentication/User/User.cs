using Microsoft.AspNetCore.Identity;
using Pathfinder.Contracts;

namespace Pathfinder.Secure.Domain.Authentication.User;

public class User : IdentityUser<int>, IUser
{
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
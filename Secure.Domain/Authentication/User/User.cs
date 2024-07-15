using Microsoft.AspNetCore.Identity;

namespace Authorization.Authentication.User
{
    public class User : IdentityUser<int>, Domain.Contracts.IUser
    {
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
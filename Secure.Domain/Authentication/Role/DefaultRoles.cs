using System.Globalization;

namespace Pathfinder.Secure.Domain.Authentication.Role;

public static class DefaultRoles
{
    public static List<Role> All()
    {
        return new List<Role>
        {
            Admin,
            Member
        };
    }

    public static readonly Role Admin = new()
    {
        Id = 1,
        Name = RoleNameForAdmin,
        NormalizedName = RoleNameForAdmin.ToUpper(CultureInfo.GetCultureInfo("en_US")),
        IsSystemDefault = true
    };

    public static readonly Role Member = new()
    {
        Id = 2,
        Name = RoleNameForMember,
        NormalizedName = RoleNameForMember.ToUpper(CultureInfo.GetCultureInfo("en_US")),
        IsSystemDefault = true
    };

    private const string RoleNameForAdmin = "Admin";
    private const string RoleNameForMember = "Member";
}
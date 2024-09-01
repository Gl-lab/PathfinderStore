using System.Globalization;

namespace Pathfinder.Secure.Domain.Authentication.User;

public static class DefaultUsers
{
    public static List<User> All()
    {
        return new List<User>
        {
            Admin,
            Member
        };
    }

    public static readonly User Admin = new()
    {
        Id = 1,
        UserName = AdminUserName,
        Email = AdminUserEmail,
        EmailConfirmed = true,
        NormalizedEmail = AdminUserEmail.ToUpper(CultureInfo.GetCultureInfo("en_US")),
        NormalizedUserName = AdminUserName.ToUpper(CultureInfo.GetCultureInfo("en_US")),
        AccessFailedCount = 5,
        PasswordHash = PasswordHashFor123Qwe
    };

    public static readonly User Member = new()
    {
        Id = 2,
        UserName = MemberUserName,
        Email = MemberUserEmail,
        EmailConfirmed = true,
        NormalizedEmail = MemberUserEmail.ToUpper(CultureInfo.GetCultureInfo("en_US")),
        NormalizedUserName = MemberUserName.ToUpper(CultureInfo.GetCultureInfo("en_US")),
        AccessFailedCount = 5,
        PasswordHash = PasswordHashFor123Qwe
    };

    private const string AdminUserName = "admin";
    private const string AdminUserEmail = "admin@mail.com";
    private const string PasswordHashFor123Qwe = "AQAAAAEAACcQAAAAEAHRoWnmspHpR/emTnFR7GuIwD1sTn/fM6O9lpdMAuagdruryhnmESp8lU2hNnEamQ=="; //123qwe
    private const string MemberUserName = "memberuser";
    private const string MemberUserEmail = "memberuser@mail.com";
        
}
using System;
using System.Globalization;
using System.Collections.Generic;

namespace Pathfinder.Core.Entities.Auth.Users
{
    public class DefaultUsers
    {
        public static List<User> All()
        {
            return new List<User>
            {
                Admin,
                Member,
                TestAdmin
            };
        }

        public static readonly User Admin = new User
        {
            Id = new Guid("C41A7761-6645-4E2C-B99D-F9E767B9AC77"),
            UserName = AdminUserName,
            Email = AdminUserEmail,
            EmailConfirmed = true,
            NormalizedEmail = AdminUserEmail.ToUpper(CultureInfo.GetCultureInfo("en_US")),
            NormalizedUserName = AdminUserName.ToUpper(CultureInfo.GetCultureInfo("en_US")),
            AccessFailedCount = 5,
            PasswordHash = PasswordHashFor123Qwe
        };

        public static readonly User Member = new User
        {
            Id = new Guid("065E903E-6F7B-42B8-B807-0C4197F9D1BC"),
            UserName = MemberUserName,
            Email = MemberUserEmail,
            EmailConfirmed = true,
            NormalizedEmail = MemberUserEmail.ToUpper(CultureInfo.GetCultureInfo("en_US")),
            NormalizedUserName = MemberUserName.ToUpper(CultureInfo.GetCultureInfo("en_US")),
            AccessFailedCount = 5,
            PasswordHash = PasswordHashFor123Qwe
        };

        public static readonly User TestAdmin = new User
        {
            Id = new Guid("4B6D9E45-626D-489A-A8CF-D32D36583AF4"),
            UserName = TestAdminUserName,
            Email = TestAdminUserEmail,
            EmailConfirmed = true,
            NormalizedEmail = TestAdminUserEmail.ToUpper(CultureInfo.GetCultureInfo("en_US")),
            NormalizedUserName = TestAdminUserName.ToUpper(CultureInfo.GetCultureInfo("en_US")),
            AccessFailedCount = 5,
            PasswordHash = PasswordHashFor123Qwe
        };

        private const string AdminUserName = "admin";
        private const string AdminUserEmail = "admin@mail.com";
        private const string PasswordHashFor123Qwe = "AQAAAAEAACcQAAAAEPZ85aLX3oCrOfmyD5aQWLzR2F53RxX0ko4ManWsiPXnbeKcL3YMOIKANLWsukGMzw=="; //123qwe
        private const string MemberUserName = "memberuser";
        private const string MemberUserEmail = "memberuser@mail.com";
        private const string TestAdminUserName = "testadmin";
        private const string TestAdminUserEmail = "testadmin@mail.com";
    }
}

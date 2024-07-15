using Authorization.Authentication.Permissions;
using Authorization.Authentication.Role;
using Authorization.Authentication.User;

namespace Secure.Infrastructure.Data
{
    public static class SeedData
    {
        #region BuildData

        public static User[] BuildApplicationUsers()
        {
            return DefaultUsers.All().ToArray();
        }

        public static Role[] BuildApplicationRoles()
        {
            return DefaultRoles.All().ToArray();
        }

        public static UserRole[] BuildApplicationUserRoles()
        {
            return new[]
            {
                //admin role to admin user
                new UserRole
                {
                    RoleId = DefaultRoles.Admin.Id,
                    UserId = DefaultUsers.Admin.Id
                },
                //member role to member user
                new UserRole
                {
                    RoleId = DefaultRoles.Member.Id,
                    UserId = DefaultUsers.Member.Id
                }
            };
        }

        public static Permission[] BuildPermissions()
        {
            return DefaultPermissions.All().ToArray();
        }

        public static RolePermission[] BuildRolePermissions()
        {
            //grant all permissions to admin role
            List<RolePermission> rolePermissions = DefaultPermissions
                                                  .All()
                                                  .ConvertAll( p => new RolePermission
                                                   {
                                                       PermissionId = p.Id,
                                                       RoleId = DefaultRoles.Admin.Id
                                                   } );

            //grant member access permission to member role
            rolePermissions.Add( new RolePermission
            {
                PermissionId = DefaultPermissions.MemberAccess.Id,
                RoleId = DefaultRoles.Member.Id
            } );

            return rolePermissions.ToArray();
        }

        #endregion
    }
}
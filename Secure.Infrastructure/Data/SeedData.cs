using Pathfinder.Secure.Domain.Authentication.Permissions;
using Pathfinder.Secure.Domain.Authentication.Role;
using Pathfinder.Secure.Domain.Authentication.User;

namespace Pathfinder.Secure.Infrastructure.Data;

public static class SeedData
{
    #region BuildData

    public static User[] BuildApplicationUsers() => DefaultUsers.All().ToArray();

    public static Role[] BuildApplicationRoles() => DefaultRoles.All().ToArray();

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

    public static Permission[] BuildPermissions() => DefaultPermissions.All().ToArray();

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
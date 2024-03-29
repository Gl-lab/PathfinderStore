using System.Collections.Generic;

namespace Pathfinder.Core.Entities.Authentication.Permissions
{
    public static class DefaultPermissions
    {
        public static List<Permission> All()
        {
            return new List<Permission>
            {
                AdministrationAccess,
                MemberAccess,
                UserRead,
                UserCreate,
                UserUpdate,
                UserDelete,
                RoleRead,
                RoleCreate,
                RoleUpdate,
                RoleDelete

                // add your permission to list
            };
        }

        public static readonly Permission AdministrationAccess = new()
        {
            DisplayName = "Administration access",
            Name = PermissionNameForAdministration,
            Id = 1, //new Guid("2A1CCB43-FA4F-48CE-B601-D3AB4D611B32")
        };

        public static readonly Permission MemberAccess = new()
        {
            DisplayName = "Member access",
            Name = PermissionNameForMemberAccess,
            Id = 2,//new Guid("28126FFD-51C2-4201-939C-B64E3DF43B9D")
        };

        public static readonly Permission UserRead = new()
        {
            DisplayName = "User read",
            Name = PermissionNameForUserRead,
            Id = 3//new Guid("86D804BD-D022-49A5-821A-D2240478AAC4")
        };

        public static readonly Permission UserCreate = new()
        {
            DisplayName = "User create",
            Name = PermissionNameForUserCreate,
            Id = 4//new Guid("8F3DE3EC-3851-4BA9-887A-2119F18AE744")
        };

        public static readonly Permission UserUpdate = new()
        {
            DisplayName = "User update",
            Name = PermissionNameForUserUpdate,
            Id = 5//new Guid("068A0171-A141-4EB2-854C-88E43EF9AB7F")
        };

        public static readonly Permission UserDelete = new()
        {
            DisplayName = "User delete",
            Name = PermissionNameForUserDelete,
            Id = 6//new Guid("70B5C5C3-2267-4F7C-B0F9-7ECC952E04A6")
        };

        public static readonly Permission RoleRead = new()
        {
            DisplayName = "Role read",
            Name = PermissionNameForRoleRead,
            Id = 7//new Guid("80562F50-8A7D-4BCD-8971-6D856BBCBB7F")
        };

        public static readonly Permission RoleCreate = new()
        {
            DisplayName = "Role create",
            Name = PermissionNameForRoleCreate,
            Id = 8//new Guid("D4D7C0E3-EFCF-4DD2-86E7-17D69FDA8C75")
        };

        public static readonly Permission RoleUpdate = new()
        {
            DisplayName = "Role update",
            Name = PermissionNameForRoleUpdate,
            Id = 9//new Guid("EA003A99-4755-4C19-B126-C5CFFBC65088")
        };

        public static readonly Permission RoleDelete = new()
        {
            DisplayName = "Role delete",
            Name = PermissionNameForRoleDelete,
            Id = 10//new Guid("8F76DE0B-114A-4DF8-A93D-CEC927D06A3C")
        };

        // add your permissions

        // these strings are using on authorize attributes
        public const string PermissionNameForAdministration = "Permissions_Administration";
        public const string PermissionNameForMemberAccess = "Permissions_Member_Access";
        public const string PermissionNameForUserRead = "Permissions_User_Read";
        public const string PermissionNameForUserCreate = "Permissions_User_Create";
        public const string PermissionNameForUserUpdate = "Permissions_User_Update";
        public const string PermissionNameForUserDelete = "Permissions_User_Delete";
        public const string PermissionNameForRoleRead = "Permissions_Role_Read";
        public const string PermissionNameForRoleCreate = "Permissions_Role_Create";
        public const string PermissionNameForRoleUpdate = "Permissions_Role_Update";
        public const string PermissionNameForRoleDelete = "Permissions_Role_Delete";

        // add your permission names
    }
}

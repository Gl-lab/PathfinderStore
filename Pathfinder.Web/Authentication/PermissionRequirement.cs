using Authorization.Authentication.Permissions;
using Microsoft.AspNetCore.Authorization;


namespace Pathfinder.Web.Authentication
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(Permission permission)
        {
            Permission = permission;
        }

        public Permission Permission { get; }
    }
}

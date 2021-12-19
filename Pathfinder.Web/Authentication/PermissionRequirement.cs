using Microsoft.AspNetCore.Authorization;
using Pathfinder.Core.Entities.Authentication.Permissions;

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

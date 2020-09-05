using Microsoft.AspNetCore.Authorization;
using Pathfinder.Core.Entities.Auth.Permissions;

namespace Pathfinder.Web.fromNucleus.Authentication
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

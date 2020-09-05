using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pathfinder.Application.Models.Auth.Permissions;
using Pathfinder.Application.Interfaces.Auth;

namespace Pathfinder.Web.fromNucleus.Authentication
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionApp;

        public PermissionHandler(IPermissionService permissionApp)
        {
            _permissionApp = permissionApp;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            var hasPermission = await _permissionApp.IsUserGrantedToPermissionAsync(context.User.Identity.Name, requirement.Permission.Name);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}

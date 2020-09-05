using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pathfinder.Application.Models.Auth.Permissions;
using Pathfinder.Application.Interfaces.Auth;

namespace Pathfinder.Web.Authentication
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService permissionApp;

        public PermissionHandler(IPermissionService permissionApp)
        {
            this.permissionApp = permissionApp;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User?.Identity.IsAuthenticated != true)
            {
                context.Fail();
                return;
            }

            var hasPermission = await permissionApp
                                    .IsUserGrantedToPermissionAsync(context.User.Identity.Name, requirement.Permission.Name)
                                    .ConfigureAwait(false);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}

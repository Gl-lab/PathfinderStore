using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Models.Auth.Permissions;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Core.Entities.Auth.Users;

namespace Pathfinder.Web.Authentication
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService permissionApp;
        private readonly IUserService userService;
        private readonly UserManager<User> userManager;

        public PermissionHandler(IPermissionService permissionApp, 
            IUserService userService, 
            UserManager<User> userManager)
        {
            this.permissionApp = permissionApp;
            this.userService = userService;
            this.userManager = userManager;
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
                //var user = await userManager.GetUserAsync(context.User).ConfigureAwait(false);
                await userService.SetCurrentUserByLogin(context.User.Identity.Name).ConfigureAwait(false);
                context.Succeed(requirement);
            }
        }
    }
}

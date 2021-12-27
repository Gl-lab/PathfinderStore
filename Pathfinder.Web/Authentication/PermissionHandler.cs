using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Core.Entities.Authentication.User;

namespace Pathfinder.Web.Authentication
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionApp;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public PermissionHandler(IPermissionService permissionApp,
            IUserService userService,
            UserManager<User> userManager)
        {
            _permissionApp = permissionApp;
            _userService = userService;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User?.Identity != null && context.User?.Identity.IsAuthenticated != true)
            {
                context.Fail();
                return;
            }

            var hasPermission = await _permissionApp
                .IsUserGrantedToPermissionAsync(context.User.Identity.Name, requirement.Permission.Name)
                .ConfigureAwait(false);
            if (hasPermission)
            {
                await _userService.SetCurrentUserByLogin(context.User.Identity.Name).ConfigureAwait(false);
                context.Succeed(requirement);
            }
        }
    }
}
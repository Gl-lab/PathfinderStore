using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pathfinder.Secure.Application.Services.Authentication;

namespace Pathfinder.Web.Authentication;

public class PermissionHandler( IPermissionService permissionApp,
                                IUserService userService )
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync( AuthorizationHandlerContext context,
                                                          PermissionRequirement requirement )
    {
        if ( context.User.Identity != null && context.User.Identity.IsAuthenticated != true )
        {
            context.Fail();
            return;
        }

        bool hasPermission = await permissionApp
                                  .IsUserGrantedToPermissionAsync( context.User.Identity?.Name,
                                       requirement.Permission.Name )
                                  .ConfigureAwait( false );
        if ( hasPermission )
        {
            await userService.SetCurrentUserByLogin( context.User.Identity.Name ).ConfigureAwait( false );
            context.Succeed( requirement );
        }
    }
}
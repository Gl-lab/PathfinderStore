using Authorization.Authentication.Permissions;
using Microsoft.AspNetCore.Authorization;


namespace Pathfinder.Web.Controllers.Base
{
    [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
    public class AuthorizedController : BaseController
    {
    }
}
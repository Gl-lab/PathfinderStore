using Microsoft.AspNetCore.Authorization;
using Pathfinder.Core.Entities.Authentication.Permissions;

namespace Pathfinder.Web.Controllers.Base
{
    [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
    public class AuthorizedController : BaseController
    {
    }
}
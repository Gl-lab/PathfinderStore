using Microsoft.AspNetCore.Authorization;
using Pathfinder.Core.Entities.Auth.Permissions;

namespace Pathfinder.Web.Controllers.Base
{
    [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
    public class AuthorizedController : BaseController
    {
    }
}
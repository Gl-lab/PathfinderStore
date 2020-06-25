using Microsoft.AspNetCore.Authorization;
using Pathfinder.Core.Entities.Auth.Permissions;

namespace Pathfinder.Web.fromNucleus.Controllers
{
    [Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
    public class AuthorizedController : BaseController
    {

    }
}
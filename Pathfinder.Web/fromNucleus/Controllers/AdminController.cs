using Microsoft.AspNetCore.Authorization;
using Pathfinder.Core.Entities.Auth.Permissions;

namespace Pathfinder.Web.fromNucleus.Controllers
{
    [Authorize(Policy = DefaultPermissions.PermissionNameForAdministration)]
    public class AdminController : BaseController
    {

    }
}
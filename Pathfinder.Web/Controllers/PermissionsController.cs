using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.Models.Auth.Permissions;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : AuthorizedController
    {
        private readonly IPermissionService permissionAppService;

        public PermissionsController(IPermissionService permissionAppService)
        {
            this.permissionAppService = permissionAppService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionModel>>> GetPermissions(string userNameOrEmail)
        {
            return Ok(await permissionAppService
                        .GetGrantedPermissionsAsync(userNameOrEmail)
                        .ConfigureAwait(false));
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Application.DTO.Auth.Roles;
using Pathfinder.Core.Entities.Auth.Permissions;
using Pathfinder.Utils.Paging;
using Pathfinder.Application.DTO.Auth;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    public class RolesController : AdminController
    {
        private readonly IRoleService roleAppService;

        public RolesController(IRoleService roleAppService)
        {
            this.roleAppService = roleAppService;
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForRoleRead)]
        public async Task<ActionResult<IPagedList<RoleListOutput>>> GetRoles([FromQuery]RoleListInput input)
        {
            return Ok(await roleAppService
                        .GetRolesAsync(input)
                       .ConfigureAwait(false));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = DefaultPermissions.PermissionNameForRoleCreate)]
        [Authorize(Policy = DefaultPermissions.PermissionNameForRoleUpdate)]
        public async Task<ActionResult<GetRoleForCreateOrUpdateOutput>> GetRoles(int id)
        {
            var getRoleForCreateOrUpdateOutput = await roleAppService
                                                    .GetRoleForCreateOrUpdateAsync(id)
                                                    .ConfigureAwait(false);

            return Ok(getRoleForCreateOrUpdateOutput);
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForRoleCreate)]
        public async Task<ActionResult> PostRoles([FromBody]CreateOrUpdateRoleInput input)
        {
            var identityResult = await roleAppService
                                    .AddRoleAsync(input)
                                    .ConfigureAwait(false);

            if (identityResult.Succeeded)
            {
                return Created(Url.Action("PostRoles"), identityResult);
            }

            return BadRequest(identityResult.Errors.Select(e => new NameValueDto(e.Code, e.Description)));
        }

        [HttpPut]
        [Authorize(Policy = DefaultPermissions.PermissionNameForRoleUpdate)]
        public async Task<ActionResult> PutRoles([FromBody]CreateOrUpdateRoleInput input)
        {
            var identityResult = await roleAppService
                                    .EditRoleAsync(input)
                                    .ConfigureAwait(false);

            if (identityResult.Succeeded)
            {
                return Ok();
            }

            return BadRequest(identityResult.Errors.Select(e => new NameValueDto(e.Code, e.Description)));
        }

        [HttpDelete]
        [Authorize(Policy = DefaultPermissions.PermissionNameForRoleDelete)]
        public async Task<ActionResult> DeleteRoles(int id)
        {
            var identityResult = await roleAppService
                                    .RemoveRoleAsync(id)
                                    .ConfigureAwait(false);

            if (identityResult.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(identityResult.Errors.Select(e => new NameValueDto(e.Code, e.Description)));
        }
    }
}
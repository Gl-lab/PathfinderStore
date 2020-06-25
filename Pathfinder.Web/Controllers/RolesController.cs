using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Application.Models.Auth.Roles;
using Pathfinder.Core.Entities.Auth.Permissions;
using Pathfinder.Core.Paging;
using Pathfinder.Application.Models.Auth;

using Pathfinder.Web.fromNucleus.Controllers;

namespace Pathfinder.Web.Controllers
{
    public class RolesController : AdminController
    {
        private readonly IRoleService _roleAppService;

        public RolesController(IRoleService roleAppService)
        {
            _roleAppService = roleAppService;
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForRoleRead)]
        public async Task<ActionResult<IPagedList<RoleListOutput>>> GetRoles([FromQuery]RoleListInput input)
        {
            return Ok(await _roleAppService.GetRolesAsync(input));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = DefaultPermissions.PermissionNameForRoleCreate)]
        [Authorize(Policy = DefaultPermissions.PermissionNameForRoleUpdate)]
        public async Task<ActionResult<GetRoleForCreateOrUpdateOutput>> GetRoles(Guid id)
        {
            var getRoleForCreateOrUpdateOutput = await _roleAppService.GetRoleForCreateOrUpdateAsync(id);

            return Ok(getRoleForCreateOrUpdateOutput);
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForRoleCreate)]
        public async Task<ActionResult> PostRoles([FromBody]CreateOrUpdateRoleInput input)
        {
            var identityResult = await _roleAppService.AddRoleAsync(input);

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
            var identityResult = await _roleAppService.EditRoleAsync(input);

            if (identityResult.Succeeded)
            {
                return Ok();
            }

            return BadRequest(identityResult.Errors.Select(e => new NameValueDto(e.Code, e.Description)));
        }

        [HttpDelete]
        [Authorize(Policy = DefaultPermissions.PermissionNameForRoleDelete)]
        public async Task<ActionResult> DeleteRoles(Guid id)
        {
            var identityResult = await _roleAppService.RemoveRoleAsync(id);

            if (identityResult.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(identityResult.Errors.Select(e => new NameValueDto(e.Code, e.Description)));
        }
    }
}
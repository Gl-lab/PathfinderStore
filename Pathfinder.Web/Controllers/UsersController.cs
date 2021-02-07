using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Utils.Paging;
using Pathfinder.Core.Entities.Auth.Permissions;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Application.Models.Auth;
using Pathfinder.Application.Models.Auth.Users;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    public class UsersController : AdminController
    {
        private readonly IUserService userAppService;

        public UsersController(IUserService userAppService)
        {
            this.userAppService = userAppService;
        }

        [HttpGet]
        [Authorize(Policy = DefaultPermissions.PermissionNameForUserRead)]
        public async Task<ActionResult<IPagedList<UserListOutput>>> GetUsers([FromQuery]UserListInput input)
        {
            return Ok(await userAppService.GetUsersAsync(input).ConfigureAwait(false));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = DefaultPermissions.PermissionNameForUserCreate)]
        [Authorize(Policy = DefaultPermissions.PermissionNameForUserUpdate)]
        public async Task<ActionResult<GetUserForCreateOrUpdateOutput>> GetUsers(int id)
        {
            var getUserForCreateOrUpdateOutput = await userAppService.GetUserForCreateOrUpdateAsync(id).ConfigureAwait(false);

            return Ok(getUserForCreateOrUpdateOutput);
        }

        [HttpPost]
        [Authorize(Policy = DefaultPermissions.PermissionNameForUserCreate)]
        public async Task<ActionResult> PostUsers([FromBody]CreateOrUpdateUserInput input)
        {
            var identityResult = await userAppService.AddUserAsync(input).ConfigureAwait(false);

            if (identityResult.Succeeded)
            {
                return Created(Url.Action("PostUsers"), identityResult);
            }

            return BadRequest(identityResult.Errors.Select(e => new NameValueDto(e.Code, e.Description)));
        }

        [HttpPut]
        [Authorize(Policy = DefaultPermissions.PermissionNameForUserCreate)]
        public async Task<ActionResult> PutUsers([FromBody]CreateOrUpdateUserInput input)
        {
            var identityResult = await userAppService.EditUserAsync(input).ConfigureAwait(false);

            if (identityResult.Succeeded)
            {
                return Ok();
            }

            return BadRequest(identityResult.Errors.Select(e => new NameValueDto(e.Code, e.Description)));
        }

        [HttpDelete]
        [Authorize(Policy = DefaultPermissions.PermissionNameForUserDelete)]
        public async Task<ActionResult> DeleteUsers(int id)
        {
            var identityResult = await userAppService.RemoveUserAsync(id).ConfigureAwait(false);

            if (identityResult.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(identityResult.Errors.Select(e => new NameValueDto(e.Code, e.Description)));
        }
    }
}
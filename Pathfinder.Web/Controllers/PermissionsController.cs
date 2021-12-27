using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.DTO.Authentication.Permissions;
using Pathfinder.Application.UseCases.Authorization.Permission;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : AuthorizedController
    {
        private readonly IMediator _mediator;

        public PermissionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions(string userNameOrEmail)
        {
            return Ok(await _mediator.Send(new PermissionsByUserNameOrEmailCommand(userNameOrEmail))
                .ConfigureAwait(false));
        }
    }
}
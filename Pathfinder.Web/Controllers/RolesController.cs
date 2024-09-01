using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Secure.Application.DTO.Authentication;
using Pathfinder.Secure.Application.DTO.Authentication.Roles;
using Pathfinder.Secure.Application.UseCases.Authorization.Roles;
using Pathfinder.Secure.Domain.Authentication.Permissions;
using Pathfinder.Utils.Paging;
using Pathfinder.Web.Controllers.Base;

namespace Pathfinder.Web.Controllers;

public class RolesController : AdminController
{
    private readonly IMediator _mediator;

    public RolesController( IMediator mediator )
    {
        _mediator = mediator;
    }

    // [HttpGet]
    // [Authorize( Policy = DefaultPermissions.PermissionNameForRoleRead )]
    // public async Task<ActionResult<IPagedList<RoleListOutput>>> GetRoles(
    //     [FromQuery] RequestRoleListCommand command )
    // {
    //     return Ok( await _mediator.Send( command ) );
    // }

    [HttpGet( "{id:int}" )]
    [Authorize( Policy = DefaultPermissions.PermissionNameForRoleCreate )]
    [Authorize( Policy = DefaultPermissions.PermissionNameForRoleUpdate )]
    public async Task<ActionResult<GetRoleForCreateOrUpdateOutput>> GetRoles( int id )
    {
        return Ok( await _mediator.Send( new RoleForCreateOrUpdateCommand( id ) ) );
    }

    [HttpPost]
    [Authorize( Policy = DefaultPermissions.PermissionNameForRoleCreate )]
    public async Task<ActionResult> PostRoles( [FromBody] CreateOrUpdateRoleCommand command )
    {
        IdentityResult identityResult = await _mediator.Send( command )
                                                       .ConfigureAwait( false );

        if ( identityResult.Succeeded )
        {
            return Created( Url.Action( "PostRoles" ) ?? String.Empty, identityResult );
        }

        return BadRequest( identityResult.Errors.Select( e => new NameValueDto( e.Code, e.Description ) ) );
    }
}
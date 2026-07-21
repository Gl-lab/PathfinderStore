using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Pathfinder.Secure.Domain.Authentication.Permissions;


namespace Pathfinder.Web.Controllers.Base;

[Authorize(Policy = DefaultPermissions.PermissionNameForMemberAccess)]
public class AuthorizedController : BaseController
{
    protected static IReadOnlyCollection<string> MapError( string message ) => [ message ];

    protected static IReadOnlyCollection<string> MapValidation( ValidationException exception ) => exception.Errors
        .Select( error => error.ErrorMessage )
        .Distinct()
        .ToArray();

    protected int CurrentUserId()
    {
        string? rawUserId = User.FindFirstValue( ClaimTypes.NameIdentifier );
        if ( !Int32.TryParse( rawUserId, out int userId ) )
        {
            throw new InvalidOperationException( "Current user identifier claim is missing." );
        }

        return userId;
    }
}

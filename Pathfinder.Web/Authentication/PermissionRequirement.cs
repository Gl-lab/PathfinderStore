using Microsoft.AspNetCore.Authorization;
using Pathfinder.Secure.Domain.Authentication.Permissions;


namespace Pathfinder.Web.Authentication;

public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(Permission permission)
    {
        Permission = permission;
    }

    public Permission Permission { get; }
}
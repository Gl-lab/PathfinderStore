using Pathfinder.Secure.Application.DTO.Authentication.Permissions;
using Pathfinder.Secure.Domain.Authentication.Permissions;

namespace Pathfinder.Secure.Application.Convertors;

public interface IPermissionsConvertor
{
    Permission Convert( PermissionDto permission );
    PermissionDto Convert( Permission permission );
}

public sealed class PermissionsConvertor : IPermissionsConvertor
{
    public Permission Convert( PermissionDto permission )
    {
        return new Permission
        {
            Id = permission.Id,
            Name = permission.Name,
            DisplayName = permission.DisplayName
        };
    }

    public PermissionDto Convert( Permission permission )
    {
        return new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name,
            DisplayName = permission.DisplayName
        };
    }
}

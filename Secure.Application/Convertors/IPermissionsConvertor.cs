using Pathfinder.Secure.Application.DTO.Authentication.Permissions;
using Pathfinder.Secure.Domain.Authentication.Permissions;

namespace Pathfinder.Secure.Application.Convertors;

public interface IPermissionsConvertor
{
    Permission Convert( PermissionDto permission );
    PermissionDto Convert( Permission permission );
}
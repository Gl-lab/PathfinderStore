using Pathfinder.Secure.Application.DTO.Authentication.Permissions;
using Pathfinder.Secure.Domain.Authentication.Permissions;

namespace Pathfinder.Secure.Application.Convertors;

public interface IPermissionsConvertor
{
    public Permission Convert( PermissionDto permission );
    public PermissionDto Convert( Permission permission );
}
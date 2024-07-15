using Authorization.Authentication.Permissions;
using Secure.Application.DTO.Authentication.Permissions;

namespace Secure.Application.Convertors;

public interface IPermissionsConvertor
{
    public Permission Convert( PermissionDto permission );
    public PermissionDto Convert( Permission permission );
}
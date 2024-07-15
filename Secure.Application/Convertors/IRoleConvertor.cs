using Authorization.Authentication.Role;
using Secure.Application.DTO.Authentication.Roles;

namespace Secure.Application.Convertors;

public interface IRoleConvertor
{
    //RoleListOutput Convert( Role role );
    RoleDto Convert( Role role );
}
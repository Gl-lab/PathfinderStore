using Pathfinder.Secure.Application.DTO.Authentication.Roles;
using Pathfinder.Secure.Domain.Authentication.Role;

namespace Pathfinder.Secure.Application.Convertors;

public interface IRoleConvertor
{
    //RoleListOutput Convert( Role role );
    RoleDto Convert( Role role );
}
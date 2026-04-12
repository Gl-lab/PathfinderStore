using Pathfinder.Secure.Application.DTO.Authentication.Roles;
using Pathfinder.Secure.Domain.Authentication.Role;

namespace Pathfinder.Secure.Application.Convertors;

public class RoleConvertor : IRoleConvertor
{
    public RoleDto Convert( Role role )
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name
        };
    }
}
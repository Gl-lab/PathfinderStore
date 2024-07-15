using Authorization.Authentication.Role;
using Secure.Application.DTO.Authentication.Roles;

namespace Secure.Application.Convertors;

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
using System.Collections.Generic;
using Pathfinder.Application.DTO.Authentication.Roles;

namespace Pathfinder.Application.DTO.Authentication.Users
{
    public class GetUserForCreateOrUpdateOutput
    {
        public UserDto User { get; init; } = new();

        public List<RoleDto> AllRoles { get; init; } = new();

        public List<int> GrantedRoleIds { get; init; } = new();
    }
}
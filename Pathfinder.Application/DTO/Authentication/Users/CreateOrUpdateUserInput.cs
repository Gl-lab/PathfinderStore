using System.Collections.Generic;

namespace Pathfinder.Application.DTO.Authentication.Users
{
    public class CreateOrUpdateUserInput
    {
        public UserDto User { get; } = new();

        public List<int> GrantedRoleIds { get; } = new();
    }
}
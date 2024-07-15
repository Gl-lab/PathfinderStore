using Secure.Application.DTO.Authentication.Roles;

namespace Secure.Application.DTO.Authentication.Users
{
    public class GetUserForCreateOrUpdateOutput
    {
        public UserDto User { get; init; } = new();

        public List<RoleDto> AllRoles { get; init; } = new();

        public List<int> GrantedRoleIds { get; init; } = new();
    }
}
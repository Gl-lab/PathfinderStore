using Pathfinder.Application.DTO.Base;

namespace Pathfinder.Application.DTO.Auth.Users
{
    public class UserDto : BaseDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
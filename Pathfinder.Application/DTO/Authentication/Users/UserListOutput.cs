using Pathfinder.Application.DTO.Base;

namespace Pathfinder.Application.DTO.Authentication.Users
{
    public class UserListOutput : BaseDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}
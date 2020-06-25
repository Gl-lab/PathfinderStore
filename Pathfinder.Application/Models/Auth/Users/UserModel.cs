using Pathfinder.Application.Models.Auth.Base;

namespace Pathfinder.Application.Models.Auth.Users
{
    public class UserModel : BaseAuthModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
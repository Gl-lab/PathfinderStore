namespace Pathfinder.Application.DTO.Authentication.Account
{
    public class LoginInput
    {
        public string UserNameOrEmail { get; set; }

        public string Password { get; set; }
    }
}
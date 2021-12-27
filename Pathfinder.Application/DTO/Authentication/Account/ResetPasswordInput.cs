namespace Pathfinder.Application.DTO.Authentication.Account
{
    public class ResetPasswordInput
    {
        public string UserNameOrEmail { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }
    }
}
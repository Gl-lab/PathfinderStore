namespace Secure.Application.DTO.Authentication.Users
{
    public class UserWithoutPasswordDto 
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}
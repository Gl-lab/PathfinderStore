using Pathfinder.Secure.Application.DTO.Authentication.Users;
using Pathfinder.Secure.Domain.Authentication.User;

namespace Pathfinder.Secure.Application.Convertors;

public interface IUserConvertor
{
    UserWithoutPasswordDto ConvertWithoutPassword( User user );
    UserDto ConvertWithPassword( User user );
}

public class UserConvertor : IUserConvertor
{
    public UserWithoutPasswordDto ConvertWithoutPassword( User user )
    {
        return new UserWithoutPasswordDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
        };
    }

    public UserDto ConvertWithPassword( User user )
    {
        return new UserDto
        {
            Email = user.Email,
            Id = user.Id,
            UserName = user.UserName,
            Password = user.PasswordHash
        };
    }
}
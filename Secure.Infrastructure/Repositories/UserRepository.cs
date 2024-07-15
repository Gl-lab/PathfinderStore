using Authorization.Authentication.User;
using Secure.Application.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Secure.Infrastructure.Repositories;

public class UserRepository: IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository( UserManager<User> userManager )
    {
        _userManager = userManager;
    }

    public async Task<User?> GetUserByNameOrEmail( string userNameOrEmail )
    {
        return await _userManager
                    .Users
                    .FirstOrDefaultAsync(u => u.UserName == userNameOrEmail || u.Email == userNameOrEmail);
    }
}
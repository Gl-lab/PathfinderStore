using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Secure.Application.Repositories;
using Pathfinder.Secure.Domain.Authentication.User;

namespace Pathfinder.Secure.Infrastructure.Repositories;

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
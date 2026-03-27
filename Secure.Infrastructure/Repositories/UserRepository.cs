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
            .Include( user => user.UserRoles )
            .ThenInclude( userRole => userRole.Role )
            .ThenInclude( role => role.RolePermissions )
            .ThenInclude( rolePermission => rolePermission.Permission )
            .FirstOrDefaultAsync( user => user.UserName == userNameOrEmail || user.Email == userNameOrEmail );
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Secure.Application.DTO.Authentication.Users;
using Pathfinder.Secure.Domain.Authentication.User;

namespace Pathfinder.Secure.Application.Services.Authentication;

public interface IUserService
{
    // Task<IPagedList<UserWithoutPasswordDto>> GetUsersAsync(UserListInput input);
    Task<GetUserForCreateOrUpdateOutput> GetUserForCreateOrUpdateAsync( int id );
    Task<IdentityResult> AddUserAsync( CreateOrUpdateUserInput input );
    Task<IdentityResult> EditUserAsync( CreateOrUpdateUserInput input );
    Task<IdentityResult> RemoveUserAsync( int id );

    Task<IdentityResult> CreateUser(
        string userName,
        string email,
        string password );

    Task<User?> FindByEmailAsync( string email );
    Task<User?> FindByNameAsync( string name );

    Task<IdentityResult> ChangePasswordAsync(
        User user,
        string currentPassword,
        string newPassword );

    Task<ClaimsIdentity?> CreateClaimsIdentityAsync( string userNameOrEmail, string password );

    Task<IdentityResult> ResetPasswordAsync(
        User user,
        string token,
        string password );

    void SetCurrentUser( User user );
    User GetCurrentUser();
    Task SetCurrentUserByLogin( string login );
}
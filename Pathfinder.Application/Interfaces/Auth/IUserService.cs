using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.DTO.Authentication.Users;
using Pathfinder.Core.Entities.Authentication.User;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.Interfaces.Auth
{
    public interface IUserService
    {
        Task<IPagedList<UserListOutput>> GetUsersAsync(UserListInput input);
        Task<GetUserForCreateOrUpdateOutput> GetUserForCreateOrUpdateAsync(int id);
        Task<IdentityResult> AddUserAsync(CreateOrUpdateUserInput input);
        Task<IdentityResult> EditUserAsync(CreateOrUpdateUserInput input);
        Task<IdentityResult> RemoveUserAsync(int id);
        Task<IdentityResult> CreateUser(string userName, string email, string password);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindByNameAsync(string name);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task<ClaimsIdentity> CreateClaimsIdentityAsync(string userNameOrEmail, string password);
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        void SetCurrentUser(User user);
        User GetCurrentUser();
        Task SetCurrentUserByLogin(string login);
    }
}
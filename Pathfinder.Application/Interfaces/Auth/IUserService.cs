using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.DTO.Auth.Users;
using Pathfinder.Core.Entities.Auth.Permissions;
using Pathfinder.Core.Entities.Auth.Users;
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

        void SetCurrentUser(User user);
        User GetCurrentUser();
        Task SetCurrentUserByLogin(string login);
    }
}
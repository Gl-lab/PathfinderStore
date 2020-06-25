using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Models.Auth.Users;
using Pathfinder.Core.Entities.Auth.Permissions;
using Pathfinder.Core.Paging;

namespace Pathfinder.Application.Interfaces.Auth
{
    public interface IUserService
    {
        Task<IPagedList<UserListOutput>> GetUsersAsync(UserListInput input);

        Task<GetUserForCreateOrUpdateOutput> GetUserForCreateOrUpdateAsync(Guid id);

        Task<IdentityResult> AddUserAsync(CreateOrUpdateUserInput input);

        Task<IdentityResult> EditUserAsync(CreateOrUpdateUserInput input);
        
        Task<IdentityResult> RemoveUserAsync(Guid id);
    }
}
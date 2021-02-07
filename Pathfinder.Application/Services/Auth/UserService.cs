using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Models.Auth.Roles;
using Pathfinder.Application.Models.Auth.Users;
using Pathfinder.Core.Entities.Auth.Users;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Utils.Paging;
using Pathfinder.Core.Repositories.Auth;

namespace Pathfinder.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly IRoleRepository roleRepository;
        private readonly IUserRoleRepository userRoleRepository;
        private User currentUser;

        public UserService(IMapper mapper,
            UserManager<User> userManager,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
        }

        public async Task<IPagedList<UserListOutput>> GetUsersAsync(UserListInput input)
        {
            var query = userManager.Users.Where(
                    predicate => predicate.UserName.Contains(input.FilteringOptions[0].Value as string) ||
                                 predicate.Email.Contains(input.FilteringOptions[0].Value as string))
                .OrderBy(input.SortingOptions?.Count > 0 ? "UserName" : input.SortingOptions[0].Field);
            var usersCount = await query.CountAsync().ConfigureAwait(false);
            IEnumerable<UserListOutput> userListOutput = mapper.Map<List<UserListOutput>>(query.ToArrayAsync().ConfigureAwait(false));
            var pageCount = usersCount/input.PageSize;
            return new PagedList<UserListOutput>(input.PageIndex, input.PageSize, usersCount, pageCount, userListOutput);
        }

        public async Task<GetUserForCreateOrUpdateOutput> GetUserForCreateOrUpdateAsync(int id)
        {
            var allRoles = mapper.Map<List<RoleModel>>(await roleRepository.GetListAsync().ConfigureAwait(false));
            var getUserForCreateOrUpdateOutput = new GetUserForCreateOrUpdateOutput
            {
                AllRoles = allRoles
            };

            if (id == 0)
            {
                return getUserForCreateOrUpdateOutput;
            }

            return await GetUserForCreateOrUpdateOutputAsync(id, allRoles).ConfigureAwait(false);
        }

        public async Task<IdentityResult> AddUserAsync(CreateOrUpdateUserInput input)
        {
            var user = new User
            {
                Id = input.User.Id,
                UserName = input.User.UserName,
                Email = input.User.Email
            };

            var createUserResult = await userManager.CreateAsync(user, input.User.Password).ConfigureAwait(false);
            if (createUserResult.Succeeded)
            {
                await GrantRolesToUserAsync(input.GrantedRoleIds, user).ConfigureAwait(false);
            }

            return createUserResult;
        }

        public async Task<IdentityResult> EditUserAsync(CreateOrUpdateUserInput input)
        {
            var user = await userManager.FindByIdAsync(input.User.Id.ToString()).ConfigureAwait(false);
            if (user.UserName == input.User.UserName && user.Id != input.User.Id)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNameAlreadyExist",
                    Description = "This user name is already exist!"
                });
            }

            if (string.IsNullOrEmpty(input.User.Password)) 
                return await UpdateUser(input, user).ConfigureAwait(false);
            var changePasswordResult = await ChangePassword(user, input.User.Password).ConfigureAwait(false);
            if (!changePasswordResult.Succeeded)
            {
                return changePasswordResult;
            }

            return await UpdateUser(input, user).ConfigureAwait(false);
        }

        public async Task<IdentityResult> RemoveUserAsync(int id)
        {
            var user = await userManager
                .Users.FirstOrDefaultAsync(u => u.Id == id)
                .ConfigureAwait(false);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found!"
                });
            }

            if (DefaultUsers.All().Select(u=>u.UserName).Contains(user.UserName))
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Code = "CannotRemoveSystemUser",
                    Description = "You cannot remove system user!"
                });
            }

            var removeUserResult = await userManager.DeleteAsync(user).ConfigureAwait(false);
            if (!removeUserResult.Succeeded)
            {
                return removeUserResult;
            }

            user.UserRoles.Clear();

            return removeUserResult;
        }

        public void SetCurrentUser(User user)
        {
            currentUser = user;
        }
        public async Task SetCurrentUserByLogin(string login)
        {
            currentUser = await userManager.FindByNameAsync(login).ConfigureAwait(false);
            //currentUser = user;
        }

        public User GetCurrentUser()
        {
            return currentUser;
        }

        private async Task GrantRolesToUserAsync(IEnumerable<int> grantedRoleIds, User user)
        {
            var userRoles = grantedRoleIds
                .Select(roleId => new UserRole {RoleId = roleId, UserId = user.Id})
                .ToList();
            await userRoleRepository.AddRangeAsync(userRoles).ConfigureAwait(false);
        }

        private async Task<IdentityResult> ChangePassword(User user, string password)
        {
            var changePasswordResult = await userManager.RemovePasswordAsync(user).ConfigureAwait(false);
            if (changePasswordResult.Succeeded)
            {
                changePasswordResult = await userManager.AddPasswordAsync(user, password).ConfigureAwait(false);
            }

            return changePasswordResult;
        }

        private async Task<IdentityResult> UpdateUser(CreateOrUpdateUserInput input, User user)
        {
            user.UserName = input.User.UserName;
            user.Email = input.User.Email;
            user.UserRoles.Clear();
            user.SecurityStamp = Guid.NewGuid().ToString();

            var updateUserResult = await userManager.UpdateAsync(user).ConfigureAwait(false);
            if (updateUserResult.Succeeded)
            {
                await GrantRolesToUserAsync(input.GrantedRoleIds, user).ConfigureAwait(false);
            }

            return updateUserResult;
        }

        private async Task<GetUserForCreateOrUpdateOutput> GetUserForCreateOrUpdateOutputAsync(int id, List<RoleModel> allRoles)
        {
            var user = await userManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            var userDto = mapper.Map<UserModel>(user);
            var grantedRoles = user.UserRoles.Select(ur => ur.Role);

            return new GetUserForCreateOrUpdateOutput
            {
                User = userDto,
                AllRoles = allRoles,
                GrantedRoleIds = grantedRoles.Select(r => r.Id).ToList()
            };
        }
    }
}

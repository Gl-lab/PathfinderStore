using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Application.DTO.Auth.Roles;
using Pathfinder.Application.DTO.Auth.Users;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Core.Entities.Auth.Users;
using Pathfinder.Core.Repositories.Auth;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private User _currentUser;

        public UserService(IMapper mapper,
            UserManager<User> userManager,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<IPagedList<UserListOutput>> GetUsersAsync(UserListInput input)
        {
            var query = _userManager.Users.Where(
                    predicate => predicate.UserName.Contains(input.FilteringOptions[0].Value as string) ||
                                 predicate.Email.Contains(input.FilteringOptions[0].Value as string))
                .OrderBy(input.SortingOptions?.Count > 0 ? "UserName" : input.SortingOptions[0].Field);
            var usersCount = await query.CountAsync().ConfigureAwait(false);
            IEnumerable<UserListOutput> userListOutput = _mapper.Map<List<UserListOutput>>(query.ToArrayAsync().ConfigureAwait(false));
            var pageCount = usersCount/input.PageSize;
            return new PagedList<UserListOutput>(input.PageIndex, input.PageSize, usersCount, pageCount, userListOutput);
        }

        public async Task<GetUserForCreateOrUpdateOutput> GetUserForCreateOrUpdateAsync(int id)
        {
            var allRoles = _mapper.Map<List<RoleDto>>(await _roleRepository.GetListAsync().ConfigureAwait(false));
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

            var createUserResult = await _userManager.CreateAsync(user, input.User.Password).ConfigureAwait(false);
            if (createUserResult.Succeeded)
            {
                await GrantRolesToUserAsync(input.GrantedRoleIds, user).ConfigureAwait(false);
            }

            return createUserResult;
        }

        public async Task<IdentityResult> EditUserAsync(CreateOrUpdateUserInput input)
        {
            var user = await _userManager.FindByIdAsync(input.User.Id.ToString()).ConfigureAwait(false);
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
            var user = await _userManager
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

            var removeUserResult = await _userManager.DeleteAsync(user).ConfigureAwait(false);
            if (!removeUserResult.Succeeded)
            {
                return removeUserResult;
            }

            user.UserRoles.Clear();

            return removeUserResult;
        }

        public void SetCurrentUser(User user)
        {
            _currentUser = user;
        }
        public async Task SetCurrentUserByLogin(string login)
        {
            _currentUser = await _userManager.FindByNameAsync(login).ConfigureAwait(false);
        }

        public User GetCurrentUser()
        {
            return _currentUser;
        }

        private async Task GrantRolesToUserAsync(IEnumerable<int> grantedRoleIds, User user)
        {
            var userRoles = grantedRoleIds
                .Select(roleId => new UserRole {RoleId = roleId, UserId = user.Id})
                .ToList();
            await _userRoleRepository.AddRangeAsync(userRoles).ConfigureAwait(false);
        }

        private async Task<IdentityResult> ChangePassword(User user, string password)
        {
            var changePasswordResult = await _userManager.RemovePasswordAsync(user).ConfigureAwait(false);
            if (changePasswordResult.Succeeded)
            {
                changePasswordResult = await _userManager.AddPasswordAsync(user, password).ConfigureAwait(false);
            }

            return changePasswordResult;
        }

        private async Task<IdentityResult> UpdateUser(CreateOrUpdateUserInput input, User user)
        {
            user.UserName = input.User.UserName;
            user.Email = input.User.Email;
            user.UserRoles.Clear();
            user.SecurityStamp = Guid.NewGuid().ToString();

            var updateUserResult = await _userManager.UpdateAsync(user).ConfigureAwait(false);
            if (updateUserResult.Succeeded)
            {
                await GrantRolesToUserAsync(input.GrantedRoleIds, user).ConfigureAwait(false);
            }

            return updateUserResult;
        }

        private async Task<GetUserForCreateOrUpdateOutput> GetUserForCreateOrUpdateOutputAsync(int id, List<RoleDto> allRoles)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            var userDto = _mapper.Map<UserDto>(user);
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

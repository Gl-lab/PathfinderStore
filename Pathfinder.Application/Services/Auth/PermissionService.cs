using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Application.Models.Auth.Permissions;
using Pathfinder.Core.Entities.Auth.Roles;
using Pathfinder.Core.Entities.Auth.Permissions;
using Pathfinder.Core.Entities.Auth.Users;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Infrastructure.Paging;

namespace Pathfinder.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public PermissionService(
            UserManager<User> userManager,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<PermissionModel>> GetGrantedPermissionsAsync(string userNameOrEmail)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u =>
                u.UserName == userNameOrEmail || u.Email == userNameOrEmail).ConfigureAwait(false);

            var grantedPermissions = user?.UserRoles
                .Select(ur => ur.Role)
                .SelectMany(r => r.RolePermissions)
                .Select(rp => rp.Permission);

            return mapper.Map<IEnumerable<PermissionModel>>(grantedPermissions);
        }

        public async Task<bool> IsUserGrantedToPermissionAsync(string userNameOrEmail, string permissionName)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u =>
                u.UserName == userNameOrEmail || u.Email == userNameOrEmail).ConfigureAwait(false);
            if (user == null)
            {
                return false;
            }

            var grantedPermissions = user.UserRoles
                .Select(ur => ur.Role)
                .SelectMany(r => r.RolePermissions)
                .Select(rp => rp.Permission);

            return grantedPermissions.Any(p => p.Name == permissionName);
        }
    }
}

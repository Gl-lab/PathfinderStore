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
        private readonly PgDbContext dbContext;

        public PermissionService(
            UserManager<User> userManager,
            IMapper mapper,
            PgDbContext dbContext)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.dbContext = dbContext;
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

        public void InitializePermissions(List<Permission> permissions)
        {
            dbContext.RolePermissions.RemoveRange(dbContext.RolePermissions.Where(rp => rp.RoleId == DefaultRoles.Admin.Id));
            dbContext.SaveChanges();

            dbContext.Permissions.RemoveRange(dbContext.Permissions);
            dbContext.SaveChanges();

            dbContext.AddRange(permissions);
            GrantAllPermissionsToAdminRole(permissions);
            dbContext.SaveChanges();
        }

        private void GrantAllPermissionsToAdminRole(List<Permission> permissions)
        {
            foreach (var permission in permissions)
            {
                dbContext.RolePermissions.Add(new RolePermission
                {
                    PermissionId = permission.Id,
                    RoleId = DefaultRoles.Admin.Id
                });
            }
        }
    }
}

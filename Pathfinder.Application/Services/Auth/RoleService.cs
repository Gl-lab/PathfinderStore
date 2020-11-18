using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Pathfinder.Application.Models.Auth.Roles;
using Pathfinder.Application.Models.Auth.Permissions;
using Pathfinder.Core.Entities.Auth.Roles;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Infrastructure.Data;
using Pathfinder.Core.Paging;
using Pathfinder.Infrastructure.Paging;

namespace  Pathfinder.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly PgDbContext dbContext;
        private readonly RoleManager<Role> roleManager;
        private readonly IMapper mapper;

        public RoleService(PgDbContext dbContext, RoleManager<Role> roleManager, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        public async Task<IPagedList<RoleListOutput>> GetRolesAsync(RoleListInput input)
        {
            var a = input.FilteringOptions?.Count > 0;
            var query = roleManager.Roles.Where(

                    predicate => predicate.Name.Contains(input.FilteringOptions[0].Value as string))
                .OrderBy(string.IsNullOrEmpty(input.SortingOptions?.First().Field) ? "Name" : input.SortingOptions?.First().Field);

            var rolesCount = await query.CountAsync().ConfigureAwait(false);
            IEnumerable<RoleListOutput> roleListOutput = mapper.Map<List<RoleListOutput>>(await query.ToArrayAsync().ConfigureAwait(false));
            int pageCount = rolesCount/input.PageSize;
            return new PagedList<RoleListOutput>(input.PageIndex, input.PageSize, rolesCount, pageCount, roleListOutput);
        }

        public async Task<GetRoleForCreateOrUpdateOutput> GetRoleForCreateOrUpdateAsync(Guid id)
        {
            var allPermissions = mapper.Map<List<PermissionModel>>(dbContext.Permissions).OrderBy(p => p.DisplayName).ToList();
            var getRoleForCreateOrUpdateOutput = new GetRoleForCreateOrUpdateOutput
            {
                AllPermissions = allPermissions
            };

            if (id == Guid.Empty)
            {
                return getRoleForCreateOrUpdateOutput;
            }

            return await GetRoleForCreateOrUpdateOutputAsync(id, allPermissions).ConfigureAwait(false);
        }

        public async Task<IdentityResult> AddRoleAsync(CreateOrUpdateRoleInput input)
        {
            var role = new Role
            {
                Id = input.Role.Id,
                Name = input.Role.Name
            };

            var createRoleResult = await roleManager.CreateAsync(role).ConfigureAwait(false);
            if (createRoleResult.Succeeded)
            {
                GrantPermissionsToRole(input.GrantedPermissionIds, role);
            }

            return createRoleResult;
        }

        public async Task<IdentityResult> EditRoleAsync(CreateOrUpdateRoleInput input)
        {
            var role = await roleManager.FindByIdAsync(input.Role.Id.ToString()).ConfigureAwait(false);
            if (role.Name == input.Role.Name && role.Id != input.Role.Id)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNameAlreadyExist",
                    Description = "This role name is already exists!"
                });
            }
            role.Name = input.Role.Name;
            role.RolePermissions.Clear();

            var updateRoleResult = await roleManager.UpdateAsync(role).ConfigureAwait(false);
            if (updateRoleResult.Succeeded)
            {
                GrantPermissionsToRole(input.GrantedPermissionIds, role);
            }

            return updateRoleResult;
        }

        public async Task<IdentityResult> RemoveRoleAsync(Guid id)
        {
            var role = await roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id).ConfigureAwait(false);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Code = "RoleNotFound",
                    Description = "Role not found!"
                });
            }

            if (role.IsSystemDefault)
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Code = "CannotRemoveSystemRole",
                    Description = "You cannot remove default system roles!"
                });
            }

            var removeRoleResult = await roleManager.DeleteAsync(role).ConfigureAwait(false);
            if (!removeRoleResult.Succeeded)
            {
                return removeRoleResult;
            }

            role.RolePermissions.Clear();
            role.UserRoles.Clear();

            return removeRoleResult;
        }

        private void GrantPermissionsToRole(IEnumerable<Guid> grantedPermissionIds, Role role)
        {
            foreach (var permissionId in grantedPermissionIds)
            {
                dbContext.RolePermissions.Add(new RolePermission
                {
                    PermissionId = permissionId,
                    RoleId = role.Id
                });
            }
        }

        private async Task<GetRoleForCreateOrUpdateOutput> GetRoleForCreateOrUpdateOutputAsync(Guid id, List<PermissionModel> allPermissions)
        {
            var role = await roleManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            var roleDto = mapper.Map<RoleModel>(role);
            var grantedPermissions = role.RolePermissions.Select(rp => rp.Permission);

            return new GetRoleForCreateOrUpdateOutput
            {
                Role = roleDto,
                AllPermissions = allPermissions,
                GrantedPermissionIds = grantedPermissions.Select(p => p.Id).ToList()
            };
        }
    }
}
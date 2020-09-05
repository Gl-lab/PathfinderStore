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
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        public RoleService(ApplicationDbContext dbContext, RoleManager<Role> roleManager, IMapper mapper)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<IPagedList<RoleListOutput>> GetRolesAsync(RoleListInput input)
        {
            var a = input.FilteringOptions?.Count > 0;
            var query = _roleManager.Roles.Where(
                    
                    predicate => predicate.Name.Contains(input.FilteringOptions.First().Value as String))
                .OrderBy(string.IsNullOrEmpty(input.SortingOptions?.First().Field) ? "Name" : input.SortingOptions?.First().Field);

            var rolesCount = await query.CountAsync();
            IEnumerable<RoleListOutput> roleListOutput = _mapper.Map<List<RoleListOutput>>(await query.ToArrayAsync());
            int pageCount = rolesCount/input.PageSize;
            return new PagedList<RoleListOutput>(input.PageIndex, input.PageSize, rolesCount, pageCount, roleListOutput);
        }

        public async Task<GetRoleForCreateOrUpdateOutput> GetRoleForCreateOrUpdateAsync(Guid id)
        {
            var allPermissions = _mapper.Map<List<PermissionModel>>(_dbContext.Permissions).OrderBy(p => p.DisplayName).ToList();
            var getRoleForCreateOrUpdateOutput = new GetRoleForCreateOrUpdateOutput
            {
                AllPermissions = allPermissions
            };

            if (id == Guid.Empty)
            {
                return getRoleForCreateOrUpdateOutput;
            }

            return await GetRoleForCreateOrUpdateOutputAsync(id, allPermissions);
        }

        public async Task<IdentityResult> AddRoleAsync(CreateOrUpdateRoleInput input)
        {
            var role = new Role
            {
                Id = input.Role.Id,
                Name = input.Role.Name
            };

            var createRoleResult = await _roleManager.CreateAsync(role);
            if (createRoleResult.Succeeded)
            {
                GrantPermissionsToRole(input.GrantedPermissionIds, role);
            }

            return createRoleResult;
        }

        public async Task<IdentityResult> EditRoleAsync(CreateOrUpdateRoleInput input)
        {
            var role = await _roleManager.FindByIdAsync(input.Role.Id.ToString());
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

            var updateRoleResult = await _roleManager.UpdateAsync(role);
            if (updateRoleResult.Succeeded)
            {
                GrantPermissionsToRole(input.GrantedPermissionIds, role);
            }

            return updateRoleResult;
        }

        public async Task<IdentityResult> RemoveRoleAsync(Guid id)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id);
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

            var removeRoleResult = await _roleManager.DeleteAsync(role);
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
                _dbContext.RolePermissions.Add(new RolePermission
                {
                    PermissionId = permissionId,
                    RoleId = role.Id
                });
            }
        }

        private async Task<GetRoleForCreateOrUpdateOutput> GetRoleForCreateOrUpdateOutputAsync(Guid id, List<PermissionModel> allPermissions)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            var roleDto = _mapper.Map<RoleModel>(role);
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Application.DTO.Auth.Permissions;
using Pathfinder.Application.DTO.Auth.Roles;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Application.UseCases.Authorization.Roles;
using Pathfinder.Core.Entities.Authentication.Role;
using Pathfinder.Core.Repositories.Auth;
using Pathfinder.Utils.Paging;

namespace  Pathfinder.Application.Services.Authentication
{
    public class RoleService : IRoleService
    {
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IRolePermissionsRepository _rolePermissionsRepository;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        public RoleService(IPermissionsRepository permissionsRepository, 
            IRolePermissionsRepository rolePermissionsRepository, 
            RoleManager<Role> roleManager, 
            IMapper mapper)
        {
            _rolePermissionsRepository = rolePermissionsRepository;
            _permissionsRepository = permissionsRepository;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<IPagedList<RoleListOutput>> GetRolesAsync(RequestRoleListCommand command)
        {
            var a = command.FilteringOptions?.Count > 0;
            var query = _roleManager.Roles.Where(

                    predicate => predicate.Name.Contains(command.FilteringOptions[0].Value as string))
                .OrderBy(string.IsNullOrEmpty(command.SortingOptions?.First().Field) ? "Name" : command.SortingOptions?.First().Field);

            var rolesCount = await query.CountAsync().ConfigureAwait(false);
            IEnumerable<RoleListOutput> roleListOutput = _mapper.Map<List<RoleListOutput>>(await query.ToArrayAsync().ConfigureAwait(false));
            var pageCount = rolesCount/command.PageSize;
            return new PagedList<RoleListOutput>(command.PageIndex, command.PageSize, rolesCount, pageCount, roleListOutput);
        }

        public async Task<GetRoleForCreateOrUpdateOutput> GetRoleForCreateOrUpdateAsync(int id)
        {
            var allPermissions = _mapper
                .Map<List<PermissionDto>>(await _permissionsRepository.GetListAsync().ConfigureAwait(false))
                .OrderBy(p => p.DisplayName)
                .ToList();
            var getRoleForCreateOrUpdateOutput = new GetRoleForCreateOrUpdateOutput
            {
                AllPermissions = allPermissions
            };

            if (id == 0)
            {
                return getRoleForCreateOrUpdateOutput;
            }

            return await GetRoleForCreateOrUpdateOutputAsync(id, allPermissions).ConfigureAwait(false);
        }

        public async Task<IdentityResult> AddRoleAsync(CreateOrUpdateRoleCommand command)
        {
            var role = new Role
            {
                Id = command.Role.Id,
                Name = command.Role.Name
            };

            var createRoleResult = await _roleManager.CreateAsync(role).ConfigureAwait(false);
            if (createRoleResult.Succeeded)
            {
                GrantPermissionsToRole(command.GrantedPermissionIds, role);
            }

            return createRoleResult;
        }

        public async Task<IdentityResult> EditRoleAsync(CreateOrUpdateRoleCommand command)
        {
            var role = await _roleManager.FindByIdAsync(command.Role.Id.ToString()).ConfigureAwait(false);
            if (role.Name == command.Role.Name && role.Id != command.Role.Id)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNameAlreadyExist",
                    Description = "This role name is already exists!"
                });
            }
            role.Name = command.Role.Name;
            role.RolePermissions.Clear();

            var updateRoleResult = await _roleManager.UpdateAsync(role).ConfigureAwait(false);
            if (updateRoleResult.Succeeded)
            {
                GrantPermissionsToRole(command.GrantedPermissionIds, role);
            }

            return updateRoleResult;
        }

        public async Task<IdentityResult> RemoveRoleAsync(int id)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id).ConfigureAwait(false);
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

            var removeRoleResult = await _roleManager.DeleteAsync(role).ConfigureAwait(false);
            if (!removeRoleResult.Succeeded)
            {
                return removeRoleResult;
            }

            role.RolePermissions.Clear();
            role.UserRoles.Clear();

            return removeRoleResult;
        }

        private void GrantPermissionsToRole(IEnumerable<int> grantedPermissionIds, Role role)
        {
            _rolePermissionsRepository
                .AddRangeAsync(grantedPermissionIds.Select(permissionId =>
                new RolePermission
                {
                    PermissionId = permissionId,
                        RoleId = role.Id
                }));
        }

        private async Task<GetRoleForCreateOrUpdateOutput> GetRoleForCreateOrUpdateOutputAsync(int id, List<PermissionDto> allPermissions)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            var roleDto = _mapper.Map<RoleDto>(role);
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
using Authorization.Authentication.Permissions;
using Authorization.Authentication.Role;
using Authorization.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pathfinder.Utils.Paging;
using Secure.Application.Convertors;
using Secure.Application.DTO.Authentication.Permissions;
using Secure.Application.DTO.Authentication.Roles;
using Secure.Application.UseCases.Authorization.Roles;

namespace Secure.Application.Services.Authentication
{
    public class RoleService : IRoleService
    {
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IRolePermissionsRepository _rolePermissionsRepository;
        private readonly RoleManager<Role> _roleManager;
        private readonly IRoleConvertor _roleConvertor;
        private readonly IPermissionsConvertor _permissionsConvertor;
      
        public RoleService(IPermissionsRepository permissionsRepository,
            IRolePermissionsRepository rolePermissionsRepository,
            RoleManager<Role> roleManager, IRoleConvertor roleConvertor, IPermissionsConvertor permissionsConvertor )
        {
            _rolePermissionsRepository = rolePermissionsRepository;
            _permissionsRepository = permissionsRepository;
            _roleManager = roleManager;
            _roleConvertor = roleConvertor;
            _permissionsConvertor = permissionsConvertor;
        }

        public async Task<IPagedList<RoleDto>> GetRolesAsync(RequestRoleListCommand command)
        {
            
            IQueryable<Role>? query = _roleManager.Roles.Where(
                                                       predicate => predicate.Name.Contains(command.FilteringOptions[0].Value as string));

            int rolesCount = await query.CountAsync().ConfigureAwait(false);
            
            IEnumerable<RoleDto> roleListOutput =
                ( await query.ToArrayAsync().ConfigureAwait( false ) ).Select( x => _roleConvertor.Convert( x ) );
                
            int pageCount = rolesCount / command.PageSize;
            return new PagedList<RoleDto>(command.PageIndex, command.PageSize, rolesCount, pageCount,
                roleListOutput);
        }

        public async Task<GetRoleForCreateOrUpdateOutput> GetRoleForCreateOrUpdateAsync(int id)
        {
            List<PermissionDto> allPermissions = 
               (await _permissionsRepository.GetListAsync().ConfigureAwait(false)).Select( x => _permissionsConvertor.Convert( x ) )
                .OrderBy(p => p.DisplayName)
                .ToList();
            GetRoleForCreateOrUpdateOutput getRoleForCreateOrUpdateOutput = new GetRoleForCreateOrUpdateOutput
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
            Role? role = new Role
            {
                Id = command.Role.Id,
                Name = command.Role.Name
            };

            IdentityResult? createRoleResult = await _roleManager.CreateAsync(role).ConfigureAwait(false);
            if (createRoleResult.Succeeded)
            {
                GrantPermissionsToRole(command.GrantedPermissionIds, role);
            }

            return createRoleResult;
        }

        public async Task<IdentityResult> EditRoleAsync(CreateOrUpdateRoleCommand command)
        {
            Role? role = await _roleManager.FindByIdAsync(command.Role.Id.ToString()).ConfigureAwait(false);
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

            IdentityResult? updateRoleResult = await _roleManager.UpdateAsync(role).ConfigureAwait(false);
            if (updateRoleResult.Succeeded)
            {
                GrantPermissionsToRole(command.GrantedPermissionIds, role);
            }

            return updateRoleResult;
        }

        public async Task<IdentityResult> RemoveRoleAsync(int id)
        {
            Role? role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id).ConfigureAwait(false);
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

            IdentityResult? removeRoleResult = await _roleManager.DeleteAsync(role).ConfigureAwait(false);
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

        private async Task<GetRoleForCreateOrUpdateOutput> GetRoleForCreateOrUpdateOutputAsync(int id,
            List<PermissionDto> allPermissions)
        {
            Role? role = await _roleManager.FindByIdAsync(id.ToString()).ConfigureAwait(false);
            RoleDto roleDto = _roleConvertor.Convert( role );
            IEnumerable<Permission> grantedPermissions = role.RolePermissions.Select(rp => rp.Permission);

            return new GetRoleForCreateOrUpdateOutput
            {
                Role = roleDto,
                AllPermissions = allPermissions,
                GrantedPermissionIds = grantedPermissions.Select(p => p.Id).ToList()
            };
        }
    }
}
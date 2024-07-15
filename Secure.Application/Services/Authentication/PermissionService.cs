using Authorization.Authentication.Permissions;
using Authorization.Authentication.User;
using Authorization.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Secure.Application.Convertors;
using Secure.Application.DTO.Authentication.Permissions;
using Secure.Application.Repositories;

namespace Secure.Application.Services.Authentication
{
    public class PermissionService : IPermissionService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionsConvertor _permissionsConvertor;

        public PermissionService(
          IUserRepository userRepository, 
          IPermissionsConvertor permissionsConvertor )
        {
            _userRepository = userRepository;
            _permissionsConvertor = permissionsConvertor;
        }

        public async Task<IEnumerable<PermissionDto>> GetGrantedPermissionsAsync(string userNameOrEmail)
        {
            User? user = await _userRepository.GetUserByNameOrEmail( userNameOrEmail );

            IEnumerable<Permission>? grantedPermissions = user?.UserRoles
                                                               .Select(ur => ur.Role)
                                                               .SelectMany(r => r.RolePermissions)
                                                               .Select(rp => rp.Permission);

            if ( grantedPermissions != null )
            {
                return grantedPermissions.Select( x => _permissionsConvertor.Convert( x ) );
            }

            throw new SecureException($"Permission not found by {userNameOrEmail}");
        }

        public async Task<bool> IsUserGrantedToPermissionAsync(string userNameOrEmail, string permissionName)
        {
            User? user = await _userRepository.GetUserByNameOrEmail( userNameOrEmail );
            if (user == null)
            {
                return false;
            }

            IEnumerable<Permission> grantedPermissions = user.UserRoles
                                                             .Select(ur => ur.Role)
                                                             .SelectMany(r => r.RolePermissions)
                                                             .Select(rp => rp.Permission);

            return grantedPermissions.Any(p => p.Name == permissionName);
        }
    }
}
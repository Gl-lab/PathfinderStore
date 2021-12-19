using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.DTO.Auth.Permissions;

namespace Pathfinder.Application.Interfaces.Auth
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetGrantedPermissionsAsync(string userNameOrEmail);
        Task<bool> IsUserGrantedToPermissionAsync(string userNameOrEmail, string permissionName);
    }
}
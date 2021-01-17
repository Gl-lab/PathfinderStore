using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.Models.Auth.Permissions;
using Pathfinder.Core.Entities.Auth.Permissions;

namespace Pathfinder.Application.Interfaces.Auth
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionModel>> GetGrantedPermissionsAsync(string userNameOrEmail);

        Task<bool> IsUserGrantedToPermissionAsync(string userNameOrEmail, string permissionName);

        Task InitializePermissions(List<Permission> permissions);
    }
}
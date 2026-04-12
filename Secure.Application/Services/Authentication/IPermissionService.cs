using Pathfinder.Secure.Application.DTO.Authentication.Permissions;

namespace Pathfinder.Secure.Application.Services.Authentication;

public interface IPermissionService
{
    Task<IEnumerable<PermissionDto>> GetGrantedPermissionsAsync(string userNameOrEmail);
    Task<bool> IsUserGrantedToPermissionAsync(string userNameOrEmail, string permissionName);
}
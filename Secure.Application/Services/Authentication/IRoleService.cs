using Microsoft.AspNetCore.Identity;
using Pathfinder.Secure.Application.DTO.Authentication.Roles;
using Pathfinder.Secure.Application.UseCases.Authorization.Roles;

namespace Pathfinder.Secure.Application.Services.Authentication;

public interface IRoleService
{
    // Task<IPagedList<RoleDto>> GetRolesAsync(RequestRoleListCommand command);

    Task<GetRoleForCreateOrUpdateOutput> GetRoleForCreateOrUpdateAsync(int id);

    Task<IdentityResult> AddRoleAsync(CreateOrUpdateRoleCommand command);

    Task<IdentityResult> EditRoleAsync(CreateOrUpdateRoleCommand command);

    Task<IdentityResult> RemoveRoleAsync(int id);
}
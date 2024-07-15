using Microsoft.AspNetCore.Identity;
using Pathfinder.Utils.Paging;
using Secure.Application.DTO.Authentication.Roles;
using Secure.Application.UseCases.Authorization.Roles;

namespace Secure.Application.Services.Authentication
{
    public interface IRoleService
    {
        Task<IPagedList<RoleDto>> GetRolesAsync(RequestRoleListCommand command);

        Task<GetRoleForCreateOrUpdateOutput> GetRoleForCreateOrUpdateAsync(int id);

        Task<IdentityResult> AddRoleAsync(CreateOrUpdateRoleCommand command);

        Task<IdentityResult> EditRoleAsync(CreateOrUpdateRoleCommand command);

        Task<IdentityResult> RemoveRoleAsync(int id);
    }
}
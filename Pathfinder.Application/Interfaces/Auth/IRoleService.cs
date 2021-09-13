using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.DTO.Auth.Roles;
using Pathfinder.Application.UseCases.Authorization.Roles;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.Interfaces.Auth
{
    public interface IRoleService
    {
        Task<IPagedList<RoleListOutput>> GetRolesAsync(RequestRoleListCommand command);

        Task<GetRoleForCreateOrUpdateOutput> GetRoleForCreateOrUpdateAsync(int id);

        Task<IdentityResult> AddRoleAsync(CreateOrUpdateRoleCommand command);

        Task<IdentityResult> EditRoleAsync(CreateOrUpdateRoleCommand command);

        Task<IdentityResult> RemoveRoleAsync(int id);
    }
}

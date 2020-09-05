using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Models.Auth.Roles;
using Pathfinder.Core.Paging;

namespace Pathfinder.Application.Interfaces.Auth
{
    public interface IRoleService
    {
        Task<IPagedList<RoleListOutput>> GetRolesAsync(RoleListInput input);

        Task<GetRoleForCreateOrUpdateOutput> GetRoleForCreateOrUpdateAsync(Guid id);

        Task<IdentityResult> AddRoleAsync(CreateOrUpdateRoleInput input);

        Task<IdentityResult> EditRoleAsync(CreateOrUpdateRoleInput input);

        Task<IdentityResult> RemoveRoleAsync(Guid id);
    }
}

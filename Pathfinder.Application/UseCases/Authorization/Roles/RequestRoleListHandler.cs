using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.DTO.Authentication.Roles;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.UseCases.Authorization.Roles
{
    public class RequestRoleListHandler : IRequestHandler<RequestRoleListCommand, IPagedList<RoleListOutput>>
    {
        private readonly IRoleService _roleService;

        public RequestRoleListHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IPagedList<RoleListOutput>> Handle(RequestRoleListCommand request,
            CancellationToken cancellationToken)
        {
            return await _roleService.GetRolesAsync(request).ConfigureAwait(false);
        }
    }
}
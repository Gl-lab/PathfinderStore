using MediatR;
using Pathfinder.Utils.Paging;
using Secure.Application.DTO.Authentication.Roles;
using Secure.Application.Services.Authentication;

namespace Secure.Application.UseCases.Authorization.Roles
{
    public class RequestRoleListHandler : IRequestHandler<RequestRoleListCommand, IPagedList<RoleDto>>
    {
        private readonly IRoleService _roleService;

        public RequestRoleListHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IPagedList<RoleDto>> Handle(RequestRoleListCommand request,
                                                      CancellationToken cancellationToken)
        {
            return await _roleService.GetRolesAsync(request).ConfigureAwait(false);
        }
    }
}
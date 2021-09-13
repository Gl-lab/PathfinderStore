using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.DTO.Auth.Permissions;
using Pathfinder.Application.Interfaces.Auth;

namespace Pathfinder.Application.UseCases.Authorization.Permission
{
    public class PermissionHandler: IRequestHandler<PermissionsByUserNameOrEmailCommand, IEnumerable<PermissionDto>>
    {
        private readonly IPermissionService _permissionService;

        public PermissionHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task<IEnumerable<PermissionDto>> Handle(PermissionsByUserNameOrEmailCommand request, CancellationToken cancellationToken)
        {
            return await _permissionService.GetGrantedPermissionsAsync(request.UserNameOrEmail);
        }
    }
}
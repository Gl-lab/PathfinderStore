using MediatR;
using Secure.Application.DTO.Authentication.Permissions;
using Secure.Application.Services.Authentication;

namespace Secure.Application.UseCases.Authorization.Permission
{
    public class PermissionHandler : IRequestHandler<PermissionsByUserNameOrEmailCommand, IEnumerable<PermissionDto>>
    {
        private readonly IPermissionService _permissionService;

        public PermissionHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task<IEnumerable<PermissionDto>> Handle(PermissionsByUserNameOrEmailCommand request,
            CancellationToken cancellationToken)
        {
            return await _permissionService.GetGrantedPermissionsAsync(request.UserNameOrEmail);
        }
    }
}
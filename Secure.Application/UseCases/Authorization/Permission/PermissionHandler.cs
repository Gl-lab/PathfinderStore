using MediatR;
using Pathfinder.Secure.Application.DTO.Authentication.Permissions;
using Pathfinder.Secure.Application.Services.Authentication;

namespace Pathfinder.Secure.Application.UseCases.Authorization.Permission;

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
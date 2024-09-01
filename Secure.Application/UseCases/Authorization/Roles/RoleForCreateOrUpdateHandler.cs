using MediatR;
using Pathfinder.Secure.Application.DTO.Authentication.Roles;
using Pathfinder.Secure.Application.Services.Authentication;

namespace Pathfinder.Secure.Application.UseCases.Authorization.Roles;

public class
    RoleForCreateOrUpdateHandler : IRequestHandler<RoleForCreateOrUpdateCommand, GetRoleForCreateOrUpdateOutput>
{
    private readonly IRoleService _roleService;

    public RoleForCreateOrUpdateHandler(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<GetRoleForCreateOrUpdateOutput> Handle(RoleForCreateOrUpdateCommand request,
                                                             CancellationToken cancellationToken)
    {
        return await _roleService
                    .GetRoleForCreateOrUpdateAsync(request.RoleId)
                    .ConfigureAwait(false);
    }
}
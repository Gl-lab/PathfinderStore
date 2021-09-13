using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.DTO.Auth.Roles;
using Pathfinder.Application.Interfaces.Auth;

namespace Pathfinder.Application.UseCases.Authorization.Roles
{
    public class RoleForCreateOrUpdateHandler:IRequestHandler<RoleForCreateOrUpdateCommand, GetRoleForCreateOrUpdateOutput>
    {
        private readonly IRoleService _roleService;

        public RoleForCreateOrUpdateHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<GetRoleForCreateOrUpdateOutput> Handle(RoleForCreateOrUpdateCommand request, CancellationToken cancellationToken)
        {
            return await _roleService
                .GetRoleForCreateOrUpdateAsync(request.RoleId)
                .ConfigureAwait(false);
        }
    }
}
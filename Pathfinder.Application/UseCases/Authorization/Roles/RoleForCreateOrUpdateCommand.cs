using MediatR;
using Pathfinder.Application.DTO.Auth.Roles;

namespace Pathfinder.Application.UseCases.Authorization.Roles
{
    public class RoleForCreateOrUpdateCommand : IRequest<GetRoleForCreateOrUpdateOutput>
    {
        public RoleForCreateOrUpdateCommand(int roleId)
        {
            RoleId = roleId;
        }

        public int RoleId { get; }
    }
}
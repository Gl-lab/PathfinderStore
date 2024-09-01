using MediatR;
using Pathfinder.Secure.Application.DTO.Authentication.Roles;

namespace Pathfinder.Secure.Application.UseCases.Authorization.Roles;

public class RoleForCreateOrUpdateCommand : IRequest<GetRoleForCreateOrUpdateOutput>
{
    public RoleForCreateOrUpdateCommand(int roleId)
    {
        RoleId = roleId;
    }

    public int RoleId { get; }
}
using MediatR;
using Pathfinder.Utils.Paging;
using Secure.Application.DTO.Authentication.Roles;

namespace Secure.Application.UseCases.Authorization.Roles
{
    public class RequestRoleListCommand : PageSearchArgs, IRequest<IPagedList<RoleDto>>
    {
        public RequestRoleListCommand() => SortingOptions.Add(new SortingOption
            { Field = "Name", Direction = SortingOption.SortingDirection.ASC });
    }
}
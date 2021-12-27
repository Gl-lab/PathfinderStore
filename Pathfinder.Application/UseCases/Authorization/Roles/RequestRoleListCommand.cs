using MediatR;
using Pathfinder.Application.DTO.Authentication.Roles;
using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.UseCases.Authorization.Roles
{
    public class RequestRoleListCommand : PageSearchArgs, IRequest<IPagedList<RoleListOutput>>
    {
        public RequestRoleListCommand() => SortingOptions.Add(new SortingOption
            { Field = "Name", Direction = SortingOption.SortingDirection.ASC });
    }
}
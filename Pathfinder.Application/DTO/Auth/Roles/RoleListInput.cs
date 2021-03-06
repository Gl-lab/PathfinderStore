using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.DTO.Auth.Roles
{
    public class RoleListInput : PageSearchArgs
    {
        public RoleListInput() => SortingOptions.Add(new SortingOption { Field ="Name", Direction = SortingOption.SortingDirection.ASC } );
    }
}
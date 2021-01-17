using Pathfinder.Core.Paging;

namespace Pathfinder.Application.Models.Auth.Roles
{
    public class RoleListInput : PageSearchArgs
    {
        public RoleListInput() => SortingOptions.Add(new SortingOption { Field ="Name", Direction = SortingOption.SortingDirection.ASC } );
    }
}
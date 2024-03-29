using Pathfinder.Utils.Paging;

namespace Pathfinder.Application.DTO.Authentication.Users
{
    public class UserListInput : PageSearchArgs
    {
        public UserListInput() => SortingOptions.Add(new SortingOption
            { Field = "UserName", Direction = SortingOption.SortingDirection.ASC });
    }
}
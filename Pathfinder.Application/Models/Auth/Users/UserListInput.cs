using Pathfinder.Core.Paging;

namespace Pathfinder.Application.Models.Auth.Users
{
    public class UserListInput : PageSearchArgs
    {
        public UserListInput()
        {
            SortingOptions.Add(new SortingOption { Field ="UserName", Direction = SortingOption.SortingDirection.ASC } );
        }
    }
}
using Pathfinder.Application.Models.Auth.Base;

namespace Pathfinder.Application.Models.Auth.Permissions
{
    public class PermissionModel : BaseAuthModel
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }
    }
}
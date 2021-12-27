using Pathfinder.Application.DTO.Base;

namespace Pathfinder.Application.DTO.Authentication.Permissions
{
    public class PermissionDto : BaseDto
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }
    }
}
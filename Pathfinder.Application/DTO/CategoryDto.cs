using Pathfinder.Application.DTO.Auth.Permissions;

namespace Pathfinder.Application.DTO
{
    public class CategoryDto : PermissionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

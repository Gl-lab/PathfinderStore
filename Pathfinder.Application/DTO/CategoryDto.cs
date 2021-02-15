using Pathfinder.Application.DTO.Auth.Permissions;
using Pathfinder.Application.DTO.Base;

namespace Pathfinder.Application.DTO
{
    public class CategoryDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

using Pathfinder.Application.DTO.Auth.Permissions;
using Pathfinder.Application.DTO.Base;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.DTO
{
    public class CategoryDto
    {
        public byte CategoryType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

using Pathfinder.Application.DTO.Base;

namespace Pathfinder.Application.DTO.Authentication.Roles
{
    public class RoleListOutput : BaseDto
    {
        public string Name { get; set; }

        public bool IsSystemDefault { get; set; }
    }
}
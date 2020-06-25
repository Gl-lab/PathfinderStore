using Pathfinder.Application.Models.Auth.Base;
namespace  Pathfinder.Application.Models.Auth.Roles
{
    public class RoleListOutput : BaseAuthModel
    {
        public string Name { get; set; }

        public bool IsSystemDefault { get; set; }
    }
}
using Pathfinder.Secure.Domain.Authentication.Role;

namespace Pathfinder.Secure.Domain.Authentication.Permissions;

public class Permission
{
    public int Id { get; set; }
    public DateTime CreationDate { get; set; }

    public DateTime ModificationDate { get; set; }

    public int CreatorId { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
namespace Pathfinder.Secure.Application.DTO.Authentication.Roles;

public class RoleListOutput
{
    public int Id { get; set; }
    public string Name { get; set; }

    public bool IsSystemDefault { get; set; }
}
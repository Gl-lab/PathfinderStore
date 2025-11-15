namespace Pathfinder.CharacterManagement.Domain.Entity;

public class Account : Utils.Entities.Base.Entity
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int UserId { get; set; }
    public DraftCharacter? DraftCharacter { get; set; }
}
using Pathfinder.Contracts;

namespace Pathfinder.CharacterManagement.Domain.Entity;

public class Account: Pathfinder.Utils.Entities.Base.Entity
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public int UserId { get; set; }
    public IUser User { get; set; }
    public List<Character> Characters { get; set; }
        
}
namespace Pathfinder.Contracts.Events;

public class UserRegisteredEvent
{
    public UserRegisteredEvent(
        int userId,
        string? name = null,
        string? surname = null )
    {
        UserId = userId;
        Name = name;
        Surname = surname;
    }

    public int UserId { get; init; }
    public string? Name { get; init; }
    public string? Surname { get; init; }
}
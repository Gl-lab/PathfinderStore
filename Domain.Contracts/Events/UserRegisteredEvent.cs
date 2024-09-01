namespace Pathfinder.Contracts.Events;

public class UserRegisteredEvent
{
    public UserRegisteredEvent( int userId )
    {
        UserId = userId;
    }

    public int UserId { get; init; }
}
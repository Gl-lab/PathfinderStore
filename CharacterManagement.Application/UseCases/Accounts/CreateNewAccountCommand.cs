using MediatR;

namespace Pathfinder.CharacterManagement.Application.UseCases.Accounts;

public class CreateNewAccountCommand : IRequest
{
    public CreateNewAccountCommand( int userId,
                                    string? name,
                                    string? surname )
    {
        UserId = userId;
        Surname = surname;
        Name = name;
    }

    public int UserId { get; init; }
    public string? Name { get; init; }
    public string? Surname { get; init; }
}
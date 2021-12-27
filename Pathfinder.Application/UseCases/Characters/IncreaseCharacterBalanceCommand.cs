using MediatR;

namespace Pathfinder.Application.UseCases.Characters;

public class IncreaseCharacterBalanceCommand : IRequest
{
    public IncreaseCharacterBalanceCommand(int value)
    {
        Value = value;
    }

    public int Value { get; }
}
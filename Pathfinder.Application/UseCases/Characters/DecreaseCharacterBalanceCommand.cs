using MediatR;

namespace Pathfinder.Application.UseCases.Characters;

public class DecreaseCharacterBalanceCommand : IRequest
{
    public DecreaseCharacterBalanceCommand(int value)
    {
        Value = value;
    }

    public int Value { get; }
}
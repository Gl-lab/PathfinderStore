using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Interfaces.Auth;
using Pathfinder.Core.UnitOfWork;

namespace Pathfinder.Application.UseCases.Characters;

public class IncreaseCharacterBalanceHandler : IRequestHandler<IncreaseCharacterBalanceCommand>
{
    private readonly IUserService _userService;
    private readonly ICharacterService _characterService;
    private readonly IUnitOfWork _unitOfWork;

    public IncreaseCharacterBalanceHandler(IUserService userService, ICharacterService characterService,
        IUnitOfWork unitOfWork)
    {
        _userService = userService;
        _characterService = characterService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(IncreaseCharacterBalanceCommand request, CancellationToken cancellationToken)
    {
        var user = _userService.GetCurrentUser();
        await _characterService.IncreaseBalance(user.Id, request.Value);
        await _unitOfWork.CommitAsync();
        return Unit.Value;
    }
}
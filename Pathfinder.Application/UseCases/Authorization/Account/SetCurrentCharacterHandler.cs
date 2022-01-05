using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.UnitOfWork;

namespace Pathfinder.Application.UseCases.Authorization.Account;

public class SetCurrentCharacterHandler : IRequestHandler<SetCurrentCharacterCommand>
{
    private readonly IAccountService _accountService;
    private readonly IUnitOfWork _unitOfWork;

    public SetCurrentCharacterHandler(IAccountService accountService, IUnitOfWork unitOfWork)
    {
        _accountService = accountService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(SetCurrentCharacterCommand request, CancellationToken cancellationToken)
    {
        await _accountService.SetCurrentCharacterAsync(request.CharacterId);
        await _unitOfWork.CommitAsync();
        return Unit.Value;
    }
}
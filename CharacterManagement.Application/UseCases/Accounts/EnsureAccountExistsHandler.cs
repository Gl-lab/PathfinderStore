using MediatR;
using Microsoft.Extensions.Logging;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CharacterManagement.Application.UseCases.Accounts;

public sealed class EnsureAccountExistsHandler : IRequestHandler<EnsureAccountExistsCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<EnsureAccountExistsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public EnsureAccountExistsHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ILogger<EnsureAccountExistsHandler> logger )
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle( EnsureAccountExistsCommand request, CancellationToken cancellationToken )
    {
        Account? account = await _accountRepository.GetByUserIdAsync( request.UserId );
        if ( account is not null )
        {
            _logger.LogInformation(
                "CharacterManagement account already exists for user {UserId}. Creation is skipped.",
                request.UserId );
            return;
        }

        Account newAccount = new Account()
        {
            UserId = request.UserId,
            Name = request.Name,
            Surname = request.Surname,
        };

        _accountRepository.Add( newAccount );
        await _unitOfWork.Commit();

        _logger.LogInformation(
            "CharacterManagement account created for user {UserId}.",
            request.UserId );
    }
}

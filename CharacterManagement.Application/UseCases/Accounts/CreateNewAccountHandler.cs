using MediatR;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CharacterManagement.Application.UseCases.Accounts;

public class CreateNewAccountHandler : IRequestHandler<CreateNewAccountCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNewAccountHandler( IAccountRepository accountRepository, IUnitOfWork unitOfWork )
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle( CreateNewAccountCommand request, CancellationToken cancellationToken )
    {
        Account? account = await _accountRepository.GetByUserIdAsync( request.UserId );
        if ( account is null )
        {
            throw new CharacterManagementException( $"Account already exists for user {request.UserId}" );
        }

        Account newAccount = new Account()
        {
            UserId = request.UserId,
            Name = request.Name,
            Surname = request.Surname,
        };
        _accountRepository.Add( newAccount );
        await _unitOfWork.Commit();
    }
}
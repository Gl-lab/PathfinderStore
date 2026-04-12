using MediatR;
using Pathfinder.CharacterManagement.Application.Builders;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class CreateCharacterHandler : IRequestHandler<CreateCharacterCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICharacterBuilder _characterBuilder;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCharacterHandler(
        IAccountRepository accountRepository,
        ICharacterBuilder characterBuilder,
        IUnitOfWork unitOfWork )
    {
        _accountRepository = accountRepository;
        _characterBuilder = characterBuilder;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle( CreateCharacterCommand request, CancellationToken cancellationToken )
    {
        Account? account = await _accountRepository.GetByUserIdAsync( request.UserId );
        if ( account is null )
        {
            throw new CharacterManagementException( $"Account was not found for user {request.UserId}." );
        }

        _characterBuilder.CreateCharacter( account.Id, request.Character.Name, request.Character.AncestryType );
        _characterBuilder.SetAncestry( request.Character.AncestryType );
        _characterBuilder.ApplyFreeBoosts( request.Character.FreeBoosts );

        DraftCharacter draftCharacter = _characterBuilder.Build();
        account.AddDraftCharacter( draftCharacter );
        await _unitOfWork.Commit();
    }
}

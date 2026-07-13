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

        AbilityType backgroundRestrictedBoost = request.Character.BackgroundRestrictedBoost
            ?? throw new CharacterManagementException( "Background restricted boost must be specified." );
        AbilityType backgroundFreeBoost = request.Character.BackgroundFreeBoost
            ?? throw new CharacterManagementException( "Background free boost must be specified." );

        _characterBuilder.CreateCharacter(
            account.Id,
            request.Character.Name,
            request.Character.AncestryType,
            request.Character.Concept,
            request.Character.Age );
        _characterBuilder.SetAncestryPackage( request.Character.HeritageId, request.Character.AncestryFeatId );
        _characterBuilder.ApplyFreeBoosts( request.Character.FreeBoosts );
        _characterBuilder.SetBackground(
            request.Character.BackgroundId,
            backgroundRestrictedBoost,
            backgroundFreeBoost );

        DraftCharacter draftCharacter = _characterBuilder.Build();
        account.AddDraftCharacter( draftCharacter );
        await _unitOfWork.Commit();
    }
}

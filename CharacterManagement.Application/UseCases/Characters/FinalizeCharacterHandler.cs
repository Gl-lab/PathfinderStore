using MediatR;
using Pathfinder.CharacterManagement.Application.Completion;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class FinalizeCharacterHandler
    : IRequestHandler<FinalizeCharacterCommand, CharacterCreationStateDto>
{
    private readonly ICharacterRepository _characterRepository;
    private readonly CharacterCompletionEvaluator _completionEvaluator;
    private readonly IUnitOfWork _unitOfWork;

    public FinalizeCharacterHandler(
        ICharacterRepository characterRepository,
        CharacterCompletionEvaluator completionEvaluator,
        IUnitOfWork unitOfWork )
    {
        _characterRepository = characterRepository;
        _completionEvaluator = completionEvaluator;
        _unitOfWork = unitOfWork;
    }

    public async Task<CharacterCreationStateDto> Handle(
        FinalizeCharacterCommand request,
        CancellationToken cancellationToken )
    {
        DraftCharacter? character = await _characterRepository.GetByIdAsync(
            request.CharacterId,
            request.UserId );
        if ( character is null )
        {
            throw new CharacterManagementException(
                $"Character {request.CharacterId} was not found for current user." );
        }

        if ( character.CreationStatus == CharacterCreationStatus.Completed )
        {
            return CreateState(
                character,
                new CharacterCompletionDto { IsComplete = true } );
        }

        CharacterCompletionDto completion = _completionEvaluator.Evaluate( character );
        if ( !completion.IsComplete )
        {
            string issueCodes = String.Join(
                ", ",
                completion.Issues.Select( issue => issue.Code ) );
            throw new Pathfinder.CharacterManagement.Domain.Exceptions.CharacterManagementException(
                $"Character creation is incomplete: {issueCodes}." );
        }

        character.FinalizeCreation( DateTimeOffset.UtcNow );
        await _unitOfWork.Commit();
        return CreateState( character, completion );
    }

    private static CharacterCreationStateDto CreateState(
        DraftCharacter character,
        CharacterCompletionDto completion ) => new CharacterCreationStateDto
        {
            CreationStatus = character.CreationStatus,
            CompletedAtUtc = character.CompletedAtUtc,
            Completion = completion,
        };
}

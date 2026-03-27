using MediatR;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class DeleteCharacterHandler : IRequestHandler<DeleteCharacterCommand>
{
    private readonly ICharacterRepository _characterRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCharacterHandler(
        ICharacterRepository characterRepository,
        IUnitOfWork unitOfWork )
    {
        _characterRepository = characterRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle( DeleteCharacterCommand request, CancellationToken cancellationToken )
    {
        DraftCharacter? draftCharacter = await _characterRepository.GetByIdAsync( request.DeletedCharacterId, request.UserId );
        if ( draftCharacter is null )
        {
            throw new CharacterManagementException( $"Character {request.DeletedCharacterId} was not found for current user." );
        }

        _characterRepository.Delete( draftCharacter );
        await _unitOfWork.Commit();
    }
}

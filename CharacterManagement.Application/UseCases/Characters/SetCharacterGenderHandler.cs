using MediatR;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.Utils.UnitOfWork;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class SetCharacterGenderHandler : IRequestHandler<SetCharacterGenderCommand>
{
    private readonly ICharacterRepository _characterRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetCharacterGenderHandler(
        ICharacterRepository characterRepository,
        IUnitOfWork unitOfWork )
    {
        _characterRepository = characterRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        SetCharacterGenderCommand request,
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

        character.SetGender( request.Gender );
        await _unitOfWork.Commit();
    }
}

using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Exceptions;
using Pathfinder.CharacterManagement.Application.Features.Characters.Queries.Mapping;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class GetCharacterByIdHandler : IRequestHandler<GetCharacterByIdCommand, CharacterDto>
{
    private readonly CharacterDetailsDtoMapper _characterDetailsDtoMapper;
    private readonly ICharacterRepository _characterRepository;

    public GetCharacterByIdHandler(
        ICharacterRepository characterRepository,
        CharacterDetailsDtoMapper characterDetailsDtoMapper )
    {
        _characterRepository = characterRepository;
        _characterDetailsDtoMapper = characterDetailsDtoMapper;
    }

    public async Task<CharacterDto> Handle( GetCharacterByIdCommand request, CancellationToken cancellationToken )
    {
        DraftCharacter? draftCharacter = await _characterRepository.GetByIdAsync( request.CharacterId, request.UserId );
        if ( draftCharacter is null )
        {
            throw new CharacterManagementException( $"Character {request.CharacterId} was not found for current user." );
        }

        return _characterDetailsDtoMapper.Convert( draftCharacter );
    }
}

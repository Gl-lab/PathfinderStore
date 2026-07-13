using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Features.Characters.Queries.Mapping;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class GetCharactersHandler : IRequestHandler<GetCharactersCommand, IReadOnlyCollection<CharacterDto>>
{
    private readonly CharacterDetailsDtoMapper _characterDetailsDtoMapper;
    private readonly ICharacterRepository _characterRepository;

    public GetCharactersHandler(
        ICharacterRepository characterRepository,
        CharacterDetailsDtoMapper characterDetailsDtoMapper )
    {
        _characterRepository = characterRepository;
        _characterDetailsDtoMapper = characterDetailsDtoMapper;
    }

    public async Task<IReadOnlyCollection<CharacterDto>> Handle( GetCharactersCommand request, CancellationToken cancellationToken )
    {
        List<DraftCharacter> draftCharacters = await _characterRepository.GetListAsync( request.UserId );

        return draftCharacters
            .Select( _characterDetailsDtoMapper.Convert )
            .ToList();
    }
}

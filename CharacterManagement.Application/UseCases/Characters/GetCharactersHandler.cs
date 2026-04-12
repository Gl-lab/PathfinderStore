using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Characters;

public sealed class GetCharactersHandler : IRequestHandler<GetCharactersCommand, IReadOnlyCollection<CharacterDto>>
{
    private readonly ICharacterConvertor _characterConvertor;
    private readonly ICharacterRepository _characterRepository;

    public GetCharactersHandler(
        ICharacterRepository characterRepository,
        ICharacterConvertor characterConvertor )
    {
        _characterRepository = characterRepository;
        _characterConvertor = characterConvertor;
    }

    public async Task<IReadOnlyCollection<CharacterDto>> Handle( GetCharactersCommand request, CancellationToken cancellationToken )
    {
        List<DraftCharacter> draftCharacters = await _characterRepository.GetListAsync( request.UserId );

        return draftCharacters
            .Select( _characterConvertor.Convert )
            .ToList();
    }
}

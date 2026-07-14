using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.ClericSpells;

public sealed class GetClericSpellsHandler : IRequestHandler<GetClericSpellsCommand, IReadOnlyCollection<SpellDefinitionDto>>
{
    private readonly ISpellRepository _spellRepository;

    public GetClericSpellsHandler( ISpellRepository spellRepository )
    {
        _spellRepository = spellRepository;
    }

    public Task<IReadOnlyCollection<SpellDefinitionDto>> Handle(
        GetClericSpellsCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<SpellDefinitionDto> spells = _spellRepository
            .GetAll()
            .Select( SpellDefinitionDtoMapper.Map )
            .ToArray();

        return Task.FromResult( spells );
    }
}

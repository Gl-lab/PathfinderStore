using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace Pathfinder.CharacterManagement.Application.UseCases.Spells;

public sealed class GetSpellOptionsHandler : IRequestHandler<GetSpellOptionsCommand, IReadOnlyCollection<SpellDefinitionDto>>
{
    private readonly ISpellRepository _spellRepository;

    public GetSpellOptionsHandler( ISpellRepository spellRepository )
    {
        _spellRepository = spellRepository;
    }

    public Task<IReadOnlyCollection<SpellDefinitionDto>> Handle(
        GetSpellOptionsCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<SpellDefinitionDto> spells = SpellCatalogResolver
            .ResolveCommonOptions(
                _spellRepository.GetAll(),
                request.Tradition,
                request.Rank,
                request.Kind )
            .Select( SpellDefinitionDtoMapper.Map )
            .ToArray();

        return Task.FromResult( spells );
    }
}

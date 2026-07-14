using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace Pathfinder.CharacterManagement.Application.UseCases.ClericSpells;

public sealed class GetClericSpellOptionsHandler : IRequestHandler<GetClericSpellOptionsCommand, ClericSpellOptionsDto>
{
    private readonly IDeityRepository _deityRepository;
    private readonly ISpellRepository _spellRepository;

    public GetClericSpellOptionsHandler(
        IDeityRepository deityRepository,
        ISpellRepository spellRepository )
    {
        _deityRepository = deityRepository;
        _spellRepository = spellRepository;
    }

    public Task<ClericSpellOptionsDto> Handle(
        GetClericSpellOptionsCommand request,
        CancellationToken cancellationToken )
    {
        Deity deity = _deityRepository.GetDeity( request.DeityId );
        IReadOnlyCollection<SpellDefinition> catalog = _spellRepository.GetAll();
        ClericSpellOptionsDto result = new ClericSpellOptionsDto
        {
            Cantrips = ClericSpellAvailabilityResolver
                .ResolveCantrips( catalog )
                .Select( ClericSpellDtoMapper.MapAvailable )
                .ToArray(),
            RankOneSpells = ClericSpellAvailabilityResolver
                .ResolveRankOneSpells( deity, catalog )
                .Select( ClericSpellDtoMapper.MapAvailable )
                .ToArray(),
        };

        return Task.FromResult( result );
    }
}

using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;
using Pathfinder.CharacterManagement.Domain.Rules.Spells;

namespace Pathfinder.CharacterManagement.Application.UseCases.ClericDomains;

public sealed class GetClericDomainsHandler : IRequestHandler<GetClericDomainsCommand, IReadOnlyCollection<ClericDomainDto>>
{
    private readonly IClericDomainRepository _clericDomainRepository;
    private readonly ISpellRepository _spellRepository;

    public GetClericDomainsHandler(
        IClericDomainRepository clericDomainRepository,
        ISpellRepository spellRepository )
    {
        _clericDomainRepository = clericDomainRepository;
        _spellRepository = spellRepository;
    }

    public Task<IReadOnlyCollection<ClericDomainDto>> Handle(
        GetClericDomainsCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<SpellDefinition> spellCatalog = _spellRepository.GetAll();
        IReadOnlyCollection<ClericDomainDto> domains = _clericDomainRepository
            .GetAll()
            .Select( domain => ClericDomainDtoMapper.Map(
                domain,
                ClericFocusPoolResolver.Resolve( domain, spellCatalog ) ) )
            .ToArray();

        return Task.FromResult( domains );
    }
}

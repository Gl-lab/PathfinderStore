using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.ClericDomains;

public sealed class GetClericDomainsHandler : IRequestHandler<GetClericDomainsCommand, IReadOnlyCollection<ClericDomainDto>>
{
    private readonly IClericDomainRepository _clericDomainRepository;

    public GetClericDomainsHandler( IClericDomainRepository clericDomainRepository )
    {
        _clericDomainRepository = clericDomainRepository;
    }

    public Task<IReadOnlyCollection<ClericDomainDto>> Handle(
        GetClericDomainsCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<ClericDomainDto> domains = _clericDomainRepository
            .GetAll()
            .Select( ClericDomainDtoMapper.Map )
            .ToArray();

        return Task.FromResult( domains );
    }
}

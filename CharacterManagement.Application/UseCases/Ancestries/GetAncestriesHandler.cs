using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Ancestries;

public sealed class GetAncestriesHandler : IRequestHandler<GetAncestriesCommand, IReadOnlyCollection<AncestryDto>>
{
    private readonly IAncestryRepository _ancestryRepository;
    private readonly IAncestryChoiceAvailabilityPolicy _ancestryChoiceAvailabilityPolicy;

    public GetAncestriesHandler(
        IAncestryRepository ancestryRepository,
        IAncestryChoiceAvailabilityPolicy? ancestryChoiceAvailabilityPolicy = null )
    {
        _ancestryRepository = ancestryRepository;
        _ancestryChoiceAvailabilityPolicy = ancestryChoiceAvailabilityPolicy ?? new CommonAncestryChoiceAvailabilityPolicy();
    }

    public Task<IReadOnlyCollection<AncestryDto>> Handle( GetAncestriesCommand request, CancellationToken cancellationToken )
    {
        IReadOnlyCollection<AncestryDto> ancestries = _ancestryRepository
            .GetAll()
            .Select( Map )
            .OrderBy( ancestry => ancestry.Name )
            .ToList();

        return Task.FromResult( ancestries );
    }

    private AncestryDto Map( Ancestry ancestry ) => AncestryDtoMapper.Map( ancestry, _ancestryChoiceAvailabilityPolicy );
}

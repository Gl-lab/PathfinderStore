using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Ancestries;

public sealed class GetAncestriesHandler : IRequestHandler<GetAncestriesCommand, IReadOnlyCollection<AncestryDto>>
{
    private readonly IAncestryRepository _ancestryRepository;

    public GetAncestriesHandler( IAncestryRepository ancestryRepository )
    {
        _ancestryRepository = ancestryRepository;
    }

    public Task<IReadOnlyCollection<AncestryDto>> Handle( GetAncestriesCommand request, CancellationToken cancellationToken )
    {
        IReadOnlyCollection<AncestryDto> ancestries = _ancestryRepository
            .GetAll()
            .Select( Map )
            .OrderBy( ancestry => ancestry.Type )
            .ToList();

        return Task.FromResult( ancestries );
    }

    private static AncestryDto Map( Ancestry ancestry )
    {
        return new AncestryDto
        {
            Type = ancestry.AncestryType,
            AbilityBoosts = ancestry.AbilityBoosts
                .Select(
                    slot => new AncestryBoostDto
                    {
                        AbilityType = slot is AncestryBoostSlot.FixedBoost fixedBoost ? fixedBoost.AbilityType : null,
                        IsFree = slot is AncestryBoostSlot.FreeBoost
                    } )
                .ToList(),
            AbilityFlaws = ancestry.AbilityFlaws.ToList(),
            BaseHitPoints = ancestry.BaseHitPoints,
            Size = ancestry.Size,
            BaseSpeed = ancestry.BaseSpeed,
            Darkvision = ancestry.Darkvision,
            LowLightVision = ancestry.LowLightVision,
        };
    }
}

using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.Deities;

public sealed class GetDeitiesHandler : IRequestHandler<GetDeitiesCommand, IReadOnlyCollection<DeityDto>>
{
    private readonly IDeityRepository _deityRepository;

    public GetDeitiesHandler( IDeityRepository deityRepository )
    {
        _deityRepository = deityRepository;
    }

    public Task<IReadOnlyCollection<DeityDto>> Handle(
        GetDeitiesCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<DeityDto> deities = _deityRepository
            .GetAll()
            .Select( DeityDtoMapper.Map )
            .ToList();

        return Task.FromResult( deities );
    }
}

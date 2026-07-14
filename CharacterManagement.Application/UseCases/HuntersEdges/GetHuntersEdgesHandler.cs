using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.HuntersEdges;

public sealed class GetHuntersEdgesHandler : IRequestHandler<GetHuntersEdgesCommand, IReadOnlyCollection<HuntersEdgeDto>>
{
    private readonly IHuntersEdgeRepository _huntersEdgeRepository;

    public GetHuntersEdgesHandler( IHuntersEdgeRepository huntersEdgeRepository )
    {
        _huntersEdgeRepository = huntersEdgeRepository;
    }

    public Task<IReadOnlyCollection<HuntersEdgeDto>> Handle(
        GetHuntersEdgesCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<HuntersEdgeDto> huntersEdges = _huntersEdgeRepository
            .GetAll()
            .OrderBy( huntersEdge => huntersEdge.Name )
            .Select( HuntersEdgeDtoMapper.Map )
            .ToArray();
        return Task.FromResult( huntersEdges );
    }
}

using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.ClericDoctrines;

public sealed class GetClericDoctrinesHandler : IRequestHandler<GetClericDoctrinesCommand, IReadOnlyCollection<ClericDoctrineDto>>
{
    private readonly IClericDoctrineRepository _clericDoctrineRepository;

    public GetClericDoctrinesHandler( IClericDoctrineRepository clericDoctrineRepository )
    {
        _clericDoctrineRepository = clericDoctrineRepository;
    }

    public Task<IReadOnlyCollection<ClericDoctrineDto>> Handle(
        GetClericDoctrinesCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<ClericDoctrineDto> doctrines = _clericDoctrineRepository
            .GetAll()
            .Select( ClericDoctrineDtoMapper.Map )
            .OrderBy( doctrine => doctrine.Name )
            .ToArray();

        return Task.FromResult( doctrines );
    }
}

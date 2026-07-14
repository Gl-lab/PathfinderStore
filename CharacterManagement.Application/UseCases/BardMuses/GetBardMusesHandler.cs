using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.BardMuses;

public sealed class GetBardMusesHandler : IRequestHandler<GetBardMusesCommand, IReadOnlyCollection<BardMuseDto>>
{
    private readonly IBardMuseRepository _bardMuseRepository;

    public GetBardMusesHandler( IBardMuseRepository bardMuseRepository )
    {
        _bardMuseRepository = bardMuseRepository;
    }

    public Task<IReadOnlyCollection<BardMuseDto>> Handle(
        GetBardMusesCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<BardMuseDto> bardMuses = _bardMuseRepository
            .GetAll()
            .OrderBy( bardMuse => bardMuse.Name )
            .Select( BardMuseDtoMapper.Map )
            .ToArray();
        return Task.FromResult( bardMuses );
    }
}

using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.Backgrounds;

public sealed class GetBackgroundsHandler : IRequestHandler<GetBackgroundsCommand, IReadOnlyCollection<BackgroundDto>>
{
    private readonly IBackgroundRepository _backgroundRepository;

    public GetBackgroundsHandler( IBackgroundRepository backgroundRepository )
    {
        _backgroundRepository = backgroundRepository;
    }

    public Task<IReadOnlyCollection<BackgroundDto>> Handle(
        GetBackgroundsCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<BackgroundDto> backgrounds = _backgroundRepository
            .GetAll()
            .Select( BackgroundDtoMapper.Map )
            .OrderBy( background => background.Name )
            .ToList();

        return Task.FromResult( backgrounds );
    }
}

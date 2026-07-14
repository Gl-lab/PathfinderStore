using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.WitchPatrons;

public sealed class GetWitchPatronsHandler
    : IRequestHandler<GetWitchPatronsCommand, IReadOnlyCollection<WitchPatronDto>>
{
    private readonly IWitchPatronRepository _witchPatronRepository;

    public GetWitchPatronsHandler( IWitchPatronRepository witchPatronRepository )
    {
        _witchPatronRepository = witchPatronRepository;
    }

    public Task<IReadOnlyCollection<WitchPatronDto>> Handle(
        GetWitchPatronsCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<WitchPatronDto> patrons = _witchPatronRepository
            .GetAll()
            .OrderBy( patron => patron.Name )
            .Select( WitchPatronDtoMapper.Map )
            .ToArray();
        return Task.FromResult( patrons );
    }
}

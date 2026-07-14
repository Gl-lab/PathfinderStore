using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.ArcaneTheses;

public sealed class GetArcaneThesesHandler
    : IRequestHandler<GetArcaneThesesCommand, IReadOnlyCollection<ArcaneThesisDto>>
{
    private readonly IArcaneThesisRepository _arcaneThesisRepository;

    public GetArcaneThesesHandler( IArcaneThesisRepository arcaneThesisRepository )
    {
        _arcaneThesisRepository = arcaneThesisRepository;
    }

    public Task<IReadOnlyCollection<ArcaneThesisDto>> Handle(
        GetArcaneThesesCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<ArcaneThesisDto> theses = _arcaneThesisRepository
            .GetAll()
            .OrderBy( thesis => thesis.Name )
            .Select( ArcaneThesisDtoMapper.Map )
            .ToArray();
        return Task.FromResult( theses );
    }
}

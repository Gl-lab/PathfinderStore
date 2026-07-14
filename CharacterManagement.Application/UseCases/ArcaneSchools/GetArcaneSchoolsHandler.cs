using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.ArcaneSchools;

public sealed class GetArcaneSchoolsHandler
    : IRequestHandler<GetArcaneSchoolsCommand, IReadOnlyCollection<ArcaneSchoolDto>>
{
    private readonly IArcaneSchoolRepository _arcaneSchoolRepository;

    public GetArcaneSchoolsHandler( IArcaneSchoolRepository arcaneSchoolRepository )
    {
        _arcaneSchoolRepository = arcaneSchoolRepository;
    }

    public Task<IReadOnlyCollection<ArcaneSchoolDto>> Handle(
        GetArcaneSchoolsCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<ArcaneSchoolDto> schools = _arcaneSchoolRepository
            .GetAll()
            .OrderBy( school => school.Name )
            .Select( ArcaneSchoolDtoMapper.Map )
            .ToArray();
        return Task.FromResult( schools );
    }
}

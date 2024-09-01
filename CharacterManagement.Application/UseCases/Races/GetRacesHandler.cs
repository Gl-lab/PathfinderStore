using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Services;
using Pathfinder.CharacterManagement.Domain.Entity;

namespace Pathfinder.CharacterManagement.Application.UseCases.Races;

public class GetRacesHandler : IRequestHandler<GetRacesCommand, ICollection<RaceDto>>
{
    private readonly IRacesService _racesService;


    public GetRacesHandler( IRacesService racesService)
    {
        _racesService = racesService;
    }

    public async Task<ICollection<RaceDto>> Handle(GetRacesCommand request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Race> races = await _racesService.RacesListAsync();
        throw new NotImplementedException();
    }
}
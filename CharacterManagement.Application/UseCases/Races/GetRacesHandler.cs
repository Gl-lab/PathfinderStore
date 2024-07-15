using CharacterManagement.Application.DTO;
using CharacterManagement.Application.Services;
using CharacterManagement.Application.UseCases.Races;
using MediatR;
using Pathfinder.Core.Entities.Account;

namespace Pathfinder.Application.UseCases.Races;

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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Interfaces;

namespace Pathfinder.Application.UseCases.Races;

public class GetRacesHandler : IRequestHandler<GetRacesCommand, ICollection<RaceDto>>
{
    private readonly IRacesService _racesService;
    private readonly IMapper _mapper;

    public GetRacesHandler(IMapper mapper, IRacesService racesService)
    {
        _mapper = mapper;
        _racesService = racesService;
    }

    public async Task<ICollection<RaceDto>> Handle(GetRacesCommand request, CancellationToken cancellationToken)
    {
        var races = await _racesService.RacesListAsync();
        return _mapper.Map<ICollection<RaceDto>>(races);
    }
}
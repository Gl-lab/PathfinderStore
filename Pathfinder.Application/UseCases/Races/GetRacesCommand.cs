using System.Collections.Generic;
using MediatR;
using Pathfinder.Application.DTO;

namespace Pathfinder.Application.UseCases.Races;

public class GetRacesCommand : IRequest<ICollection<RaceDto>>
{
}
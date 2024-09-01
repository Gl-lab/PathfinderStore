using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.Races;

public class GetRacesCommand : IRequest<ICollection<RaceDto>>
{
}
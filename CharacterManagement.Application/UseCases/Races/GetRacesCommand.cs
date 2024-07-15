using CharacterManagement.Application.DTO;
using MediatR;

namespace CharacterManagement.Application.UseCases.Races;

public class GetRacesCommand : IRequest<ICollection<RaceDto>>
{
}
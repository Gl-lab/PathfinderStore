using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.ClericDomains;

public sealed class GetClericDomainsCommand : IRequest<IReadOnlyCollection<ClericDomainDto>>
{
}

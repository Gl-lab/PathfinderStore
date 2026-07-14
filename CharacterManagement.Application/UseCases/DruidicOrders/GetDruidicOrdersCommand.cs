using MediatR;
using Pathfinder.CharacterManagement.Application.DTO;

namespace Pathfinder.CharacterManagement.Application.UseCases.DruidicOrders;

public sealed class GetDruidicOrdersCommand : IRequest<IReadOnlyCollection<DruidicOrderDto>>
{
}

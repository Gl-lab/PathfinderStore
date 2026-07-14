using MediatR;
using Pathfinder.CharacterManagement.Application.Converters;
using Pathfinder.CharacterManagement.Application.DTO;
using Pathfinder.CharacterManagement.Application.Repositories;

namespace Pathfinder.CharacterManagement.Application.UseCases.DruidicOrders;

public sealed class GetDruidicOrdersHandler : IRequestHandler<GetDruidicOrdersCommand, IReadOnlyCollection<DruidicOrderDto>>
{
    private readonly IDruidicOrderRepository _druidicOrderRepository;

    public GetDruidicOrdersHandler( IDruidicOrderRepository druidicOrderRepository )
    {
        _druidicOrderRepository = druidicOrderRepository;
    }

    public Task<IReadOnlyCollection<DruidicOrderDto>> Handle(
        GetDruidicOrdersCommand request,
        CancellationToken cancellationToken )
    {
        IReadOnlyCollection<DruidicOrderDto> druidicOrders = _druidicOrderRepository
            .GetAll()
            .OrderBy( druidicOrder => druidicOrder.Name )
            .Select( DruidicOrderDtoMapper.Map )
            .ToArray();
        return Task.FromResult( druidicOrders );
    }
}

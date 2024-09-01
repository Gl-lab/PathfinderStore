using System.Collections.Generic;
using MediatR;
using Pathfinder.Store.Application.DTO.Shop;

namespace Pathfinder.Store.Application.UseCases.Shop;

public class GetAllShopsCommand : IRequest<IReadOnlyList<ShopDto>>
{
    public GetAllShopsCommand()
    {
    }
}
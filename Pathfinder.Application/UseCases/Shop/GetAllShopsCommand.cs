using MediatR;
using Pathfinder.Application.DTO.Shop;
using System.Collections.Generic;

namespace Pathfinder.Application.UseCases.Shop
{
    public class GetAllShopsCommand : IRequest<IReadOnlyList<ShopDto>>
    {
        public GetAllShopsCommand()
        {
        }
    }
}

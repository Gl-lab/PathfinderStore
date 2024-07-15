using System;
using MediatR;
using Pathfinder.Application.DTO.Shop;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pathfinder.Application.Services;

namespace Pathfinder.Application.UseCases.Shop
{
    internal class GetAllShopHandler : IRequestHandler<GetAllShopsCommand, IReadOnlyList<ShopDto>>
    {
        private readonly IShopService _shopService;

        public GetAllShopHandler( IShopService shopService )
        {
            _shopService = shopService;
        }

        public async Task<IReadOnlyList<ShopDto>> Handle( GetAllShopsCommand request,
                                                          CancellationToken cancellationToken )
        {
            IReadOnlyList<Core.Entities.Shop.Shop> shops = await _shopService.GetShopList();
            throw new NotImplementedException();
            //return _mapper.Map<IReadOnlyList<ShopDto>>( shops );
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Store.Application.DTO.Shop;
using Pathfinder.Store.Application.Services;

namespace Pathfinder.Store.Application.UseCases.Shop;

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
        IReadOnlyList<Contracts.Core.Entities.Shop.Shop> shops = await _shopService.GetShopList();
        throw new NotImplementedException();
        //return _mapper.Map<IReadOnlyList<ShopDto>>( shops );
    }
}
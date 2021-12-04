using AutoMapper;
using MediatR;
using Pathfinder.Application.DTO.Shop;
using Pathfinder.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pathfinder.Application.UseCases.Shop
{
    internal class GetAllShopHandler : IRequestHandler<GetAllShopsCommand, IReadOnlyList<ShopDto>>
    {
        private readonly IShopService _shopService;
        private readonly IMapper _mapper;


        public GetAllShopHandler(IShopService shopService, IMapper mapper)
        {
            _shopService = shopService;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<ShopDto>> Handle(GetAllShopsCommand request, CancellationToken cancellationToken)
        {
            var shops = await _shopService.GetShopList();
            return _mapper.Map<IReadOnlyList<ShopDto>>(shops);
        }
    }
}

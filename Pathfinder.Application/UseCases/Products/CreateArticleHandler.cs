using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.Entities.Product;
using Pathfinder.Core.UnitOfWork;

namespace Pathfinder.Application.UseCases.Products
{
    public class CreateArticleHandler : IRequestHandler<CreateArticleCommand, ProductDto>
    {
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateArticleHandler(IProductService productService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _productService = productService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            Product product;
            try
            {
                product = await _productService.CreateArticle(request.Name, request.Description, request.Price,
                    request.Weight,
                    request.CategoryType);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return _mapper.Map<ProductDto>(product);
        }
    }
}
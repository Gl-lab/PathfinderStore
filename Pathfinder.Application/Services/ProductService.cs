using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.Mapper;
using Pathfinder.Application.Models;
using Pathfinder.Core.Entities;
using Pathfinder.Core.Interfaces;
using Pathfinder.Core.Paging;
using Pathfinder.Core.Repositories;
using Pathfinder.Infrastructure.Paging;
using AutoMapper;

namespace Pathfinder.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        //private readonly IAppLogger<ProductService> logger;
        private readonly IMapper mapper;

        public ProductService(IProductRepository productRepository,/* IAppLogger<ProductService> logger,*/ IMapper mapper)
        {
            this.productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
          //  this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ProductModel>> GetProductList()
        {
            var productList = await productRepository.ListAllAsync().ConfigureAwait(false);

            var productModels = mapper.Map<IEnumerable<ProductModel>>(productList);

            return productModels;
        }

        public async Task<IPagedList<ProductModel>> SearchProducts(PageSearchArgs args)
        {
            var productPagedList = await productRepository.SearchProductsAsync(args).ConfigureAwait(false);
            var productModels = mapper.Map<List<ProductModel>>(productPagedList.Items);

            var productModelPagedList = new PagedList<ProductModel>(
                productPagedList.PageIndex,
                productPagedList.PageSize,
                productPagedList.TotalCount,
                productPagedList.TotalPages,
                productModels);

            return productModelPagedList;
        }

        public async Task<ProductModel> GetProductById(int productId)
        {
            var product = await productRepository.GetByIdAsync(productId).ConfigureAwait(false);

            var productModel = mapper.Map<ProductModel>(product);

            return productModel;
        }

        public async Task<IEnumerable<ProductModel>> GetProductsByName(string name)
        {
            var productList = await productRepository.GetProductByNameAsync(name).ConfigureAwait(false);
            var productModels = mapper.Map<IEnumerable<ProductModel>>(productList);
            return productModels;
        }

        public async Task<IEnumerable<ProductModel>> GetProductsByCategoryId(int categoryId)
        {
            var productList = await productRepository.GetProductByCategoryAsync(categoryId).ConfigureAwait(false);

            return mapper.Map<IEnumerable<ProductModel>>(productList);
        }

        public async Task<ProductModel> CreateProduct(ProductModel product)
        {
            var existingProduct = await productRepository.GetByIdAsync(product.Id).ConfigureAwait(false);
            if (existingProduct != null)
            {
                throw new ApplicationException("Product with this id already exists");
            }

            var newProduct = mapper.Map<Product>(product);
            newProduct = await productRepository.SaveAsync(newProduct).ConfigureAwait(false);

           // logger.LogInformation("Entity successfully added - AspnetRunAppService");

            var newProductModel = mapper.Map<ProductModel>(newProduct);
            return newProductModel;
        }

        public async Task UpdateProduct(ProductModel product)
        {
            var existingProduct = await productRepository.GetByIdAsync(product.Id).ConfigureAwait(false);
            if (existingProduct == null)
            {
                throw new ApplicationException("Product with this id is not exists");
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.CategoryId = product.CategoryId;

            await productRepository.SaveAsync(existingProduct).ConfigureAwait(false);

            //logger.LogInformation("Entity successfully updated - AspnetRunAppService");
        }

        public async Task DeleteProductById(int productId)
        {
            var existingProduct = await productRepository.GetByIdAsync(productId).ConfigureAwait(false);
            if (existingProduct == null)
            {
                throw new ApplicationException("Product with this id is not exists");
            }

            await productRepository.DeleteAsync(existingProduct).ConfigureAwait(false);

            //logger.LogInformation("Entity successfully deleted - AspnetRunAppService");
        }
    }
}

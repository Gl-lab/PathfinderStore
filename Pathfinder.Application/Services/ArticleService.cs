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
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository productRepository;
        //private readonly IAppLogger<ArticleService> logger;
        private readonly IMapper mapper;

        public ArticleService(IArticleRepository productRepository,/* IAppLogger<ArticleService> logger,*/ IMapper mapper)
        {
            this.productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
          //  this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ArticleModel>> GetArticleList()
        {
            var productList = await productRepository
                .ListAllAsync()
                .ConfigureAwait(false);

            var productModels = mapper.Map<IEnumerable<ArticleModel>>(productList);

            return productModels;
        }

        public async Task<IPagedList<ArticleModel>> SearchArticles(PageSearchArgs args)
        {
            var productPagedList = await productRepository.SearchAsync(args).ConfigureAwait(false);
            var productModels = mapper.Map<List<ArticleModel>>(productPagedList.Items);

            return new PagedList<ArticleModel>(
                productPagedList.PageIndex,
                productPagedList.PageSize,
                productPagedList.TotalCount,
                productPagedList.TotalPages,
                productModels);
        }

        public async Task<ArticleModel> GetArticleById(int productId)
        {
            var product = await productRepository.GetByIdAsync(productId).ConfigureAwait(false);

            var productModel = mapper.Map<ArticleModel>(product);

            return productModel;
        }

        public async Task<IEnumerable<ArticleModel>> GetArticlesByName(string name)
        {
            var productList = await productRepository.GetListByNameAsync(name).ConfigureAwait(false);
            var productModels = mapper.Map<IEnumerable<ArticleModel>>(productList);
            return productModels;
        }

        public async Task<IEnumerable<ArticleModel>> GetArticlesByCategoryId(int categoryId)
        {
            var productList = await productRepository.GetListByCategoryAsync(categoryId).ConfigureAwait(false);

            return mapper.Map<IEnumerable<ArticleModel>>(productList);
        }

        public async Task<ArticleModel> CreateArticle(ArticleModel product)
        {
            var existingArticle = await productRepository.GetByIdAsync(product.Id).ConfigureAwait(false);
            if (existingArticle != null)
            {
                throw new ApplicationException("Article with this id already exists");
            }

            var newArticle = mapper.Map<Article>(product);
            newArticle = await productRepository.SaveAsync(newArticle).ConfigureAwait(false);

           // logger.LogInformation("Entity successfully added - AspnetRunAppService");

            var newArticleModel = mapper.Map<ArticleModel>(newArticle);
            return newArticleModel;
        }

        public async Task UpdateArticle(ArticleModel product)
        {
            var existingArticle = await productRepository.GetByIdAsync(product.Id).ConfigureAwait(false);
            if (existingArticle == null)
            {
                throw new ApplicationException("Article with this id is not exists");
            }

            existingArticle.Name = product.Name;
            existingArticle.Description = product.Description;
            existingArticle.Price = product.Price;
            existingArticle.CategoryId = product.CategoryId;

            await productRepository.SaveAsync(existingArticle).ConfigureAwait(false);

            //logger.LogInformation("Entity successfully updated - AspnetRunAppService");
        }

        public async Task DeleteArticleById(int productId)
        {
            var existingArticle = await productRepository.GetByIdAsync(productId).ConfigureAwait(false);
            if (existingArticle == null)
            {
                throw new ApplicationException("Article with this id is not exists");
            }

            await productRepository.DeleteAsync(existingArticle).ConfigureAwait(false);

            //logger.LogInformation("Entity successfully deleted - AspnetRunAppService");
        }
    }
}

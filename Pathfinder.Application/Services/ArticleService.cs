using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinder.Application.Interfaces;
using Pathfinder.Application.DTO;
using Pathfinder.Core.Repositories;
using Pathfinder.Utils.Paging;
using AutoMapper;
using Pathfinder.Core.Entities.Product;

namespace Pathfinder.Application.Services
{
    public sealed class ArticleService : IArticleService
    {
        private readonly IArticleRepository _productRepository;
        private readonly IMapper _mapper;

        public ArticleService(IArticleRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ArticleDto>> GetArticleList()
        {
            var productList = await _productRepository
                .ListAllAsync()
                .ConfigureAwait(false);

            var productModels = _mapper.Map<IEnumerable<ArticleDto>>(productList);

            return productModels;
        }

        public async Task<IPagedList<ArticleDto>> SearchArticles(PageSearchArgs args)
        {
            var productPagedList = await _productRepository.SearchAsync(args).ConfigureAwait(false);
            var productModels = _mapper.Map<List<ArticleDto>>(productPagedList.Items);

            return new PagedList<ArticleDto>(
                productPagedList.PageIndex,
                productPagedList.PageSize,
                productPagedList.TotalCount,
                productPagedList.TotalPages,
                productModels);
        }

        public async Task<ArticleDto> GetArticleById(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId).ConfigureAwait(false);

            var productModel = _mapper.Map<ArticleDto>(product);

            return productModel;
        }

        public async Task<IEnumerable<ArticleDto>> GetArticlesByName(string name)
        {
            var productList = await _productRepository.SearchByNameAsync(name).ConfigureAwait(false);
            var productModels = _mapper.Map<IEnumerable<ArticleDto>>(productList);
            return productModels;
        }

        public async Task<IEnumerable<ArticleDto>> GetArticlesByCategoryId(CategoryType categoryType)
        {
            var productList = await _productRepository
                .GetListByCategoryAsync(categoryType)
                .ConfigureAwait(false);

            return _mapper.Map<IEnumerable<ArticleDto>>(productList);
        }
        
        public async Task<ArticleDto> CreateArticle(string name, string description, decimal? price, decimal? weight, byte categoryType)
        {
            var existingArticle = await _productRepository.GetByName(name).ConfigureAwait(false);
            if (existingArticle != null)
            {
                throw new ApplicationException("Article with this id already exists");
            }

            if (!Enum.IsDefined(typeof(CategoryType), categoryType))
            {
                throw new ApplicationException("categoryType is undefined");
            }

            var newArticle = new Article
            {
                Name = name,
                Description = description,
                Price = price,
                Weight = weight,
                CategoryType = (CategoryType) categoryType
            };

            _productRepository.Add(newArticle);

            var newArticleModel = _mapper.Map<ArticleDto>(newArticle);
            return newArticleModel;
        }
        
        public async Task<ArticleDto> CreateArticle(ArticleDto product)
        {
            var existingArticle = await _productRepository.GetByIdAsync(product.Id).ConfigureAwait(false);
            if (existingArticle != null)
            {
                throw new ApplicationException("Article with this id already exists");
            }

            var newArticle = _mapper.Map<Article>(product);
            newArticle = await _productRepository.SaveAsync(newArticle).ConfigureAwait(false);

            var newArticleModel = _mapper.Map<ArticleDto>(newArticle);
            return newArticleModel;
        }

        public async Task UpdateArticle(ArticleDto product)
        {
            var existingArticle = await _productRepository.GetByIdAsync(product.Id).ConfigureAwait(false);
            if (existingArticle == null)
            {
                throw new ApplicationException("Article with this id is not exists");
            }

            existingArticle.Name = product.Name;
            existingArticle.Description = product.Description;
            existingArticle.Price = product.Price;
            existingArticle.CategoryType = (CategoryType)product.CategoryType;

            _productRepository.Add(existingArticle);
        }

        public async Task DeleteArticleById(int productId)
        {
            var existingArticle = await _productRepository.GetByIdAsync(productId).ConfigureAwait(false);
            if (existingArticle == null)
            {
                throw new ApplicationException("Article with this id is not exists");
            }

            await _productRepository.DeleteAsync(existingArticle).ConfigureAwait(false);
        }
    }
}

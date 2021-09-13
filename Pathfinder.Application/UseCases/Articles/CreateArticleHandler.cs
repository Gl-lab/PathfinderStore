using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Pathfinder.Application.DTO;
using Pathfinder.Application.Interfaces;
using Pathfinder.Core.UnitOfWork;

namespace Pathfinder.Application.UseCases.Articles
{
    public class CreateArticleHandler: IRequestHandler<CreateArticleCommand, ArticleDto>
    {
        private readonly IArticleService _articleServiceService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateArticleHandler(IArticleService articleServiceService, IUnitOfWork unitOfWork)
        {
            _articleServiceService = articleServiceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ArticleDto> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            ArticleDto article;
            try
            {
                article = await _articleServiceService.CreateArticle(request.Name, request.Description, request.Price, request.Weight,
                    request.CategoryType);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            return article;
        }
    }
}
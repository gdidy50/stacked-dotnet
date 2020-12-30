using System;
using System.Threading.Tasks;
using Stacked.Data;
using Stacked.Data.Models;
using Stacked.Models;
using Stacked.Services.Interfaces;
using Stacked.Services.Models;

namespace Stacked.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IRepository<Article> _articleRepository;

        public ArticleService(IRepository<Article> articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public Task<PagedServiceResult<ArticleDto>> GetAll(int page, int perPage)
        {
            throw new System.NotImplementedException();
        }

        public Task<ServiceResult<ArticleDto>> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<ArticleDto>> Update(Guid id, ArticleDto article)
        {
            throw new NotImplementedException();
        }
        public Task<ServiceResult<Guid>> Create(ArticleDto article)
        {
            throw new NotImplementedException();
        }
        public Task<ServiceResult<Guid>> Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
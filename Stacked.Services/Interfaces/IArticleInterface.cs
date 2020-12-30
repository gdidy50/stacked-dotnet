using System;
using System.Threading.Tasks;
using Stacked.Models;
using Stacked.Services.Models;

namespace Stacked.Services.Interfaces
{
    public interface IArticleService
    {
        Task<PagedServiceResult<ArticleDto>> GetAll(int page, int perPage);
        Task<ServiceResult<ArticleDto>> GetById(Guid id);
        Task<ServiceResult<ArticleDto>> Update(Guid id, ArticleDto article);
        Task<ServiceResult<Guid>> Create(ArticleDto article);
        Task<ServiceResult<Guid>> Delete(Guid id);

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Stacked.Data;
using Stacked.Data.Models;
using Stacked.Models;
using Stacked.Services.Interfaces;
using Stacked.Services.Models;

namespace Stacked.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IRepository<Article> _articles;
        private readonly IRepository<ArticleTag> _articleTags;
        private readonly IRepository<Tag> _tags;
        private readonly IRepository<User> _users;
        private readonly ILogger<ArticleService> _logger;
        private readonly IMapper _mapper;

        public ArticleService(
            IRepository<Article> articles,
            IRepository<ArticleTag> articleTags,
            IRepository<Tag> tags,
            IRepository<User> users,
            ILogger<ArticleService> logger,
            IMapper mapper)
        {
            _articles = articles;
            _articleTags = articleTags;
            _tags = tags;
            _users = users;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PagedServiceResult<ArticleDto>> GetAll(int page, int perPage)
        {
            try
            {
                var articles = await _articles.GetAll(page, perPage);
                var articleModels = new List<ArticleDto>();

                foreach (var article in articles.Results)
                {
                    var articleModel = _mapper.Map<ArticleDto>(article);
                    // Join table
                    var articleTags = await _articleTags.GetAllWhere(
                        at => at.ArticleId == article.Id,
                        at => at.CreatedOn
                    );
                    // Get the associated Tag Ids
                    var tagIds = articleTags.Select(t => t.TagId).ToList();
                    // Get the Tag entity instances
                    var tags = await _tags.GetAllWhere(
                        tag => tagIds.Contains(tag.Id),
                        tag => tag.CreatedOn
                    );

                    if (articleTags.Count > 0)
                        articleModel.Tags = tags.Select(t => t.Name).ToList();

                    // Find and assign Author User
                    var author = await _users
                        .GetFirstWhere(u => u.Id == article.AuthorId, u => u.UpdatedOn);
                    articleModel.AuthorName = author.UserName;

                    articleModels.Add(articleModel);
                }

                var articlesResultModel = new PaginationResult<ArticleDto>
                {
                    TotalCount = articles.TotalCount,
                    Results = articleModels,
                    ResultsPerPage = articles.ResultsPerPage,
                    PageNumber = articles.PageNumber
                };

                _logger.LogDebug($"Returning paginated articles: page {page}, perPage {perPage}");

                return new PagedServiceResult<ArticleDto>
                {
                    IsSuccess = true,
                    Data = articlesResultModel,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to return paginated articles: {ex.StackTrace}");
                return new PagedServiceResult<ArticleDto>
                {
                    IsSuccess = false,
                    Data = null,
                    Error = new ServiceError { StackTrace = ex.StackTrace, Message = ex.Message }
                };
            }
        }

        public async Task<ServiceResult<ArticleDto>> GetById(Guid id)
        {
            try
            {
                var article = await _articles.GetById(id);

                if (article == null)
                {
                    return new ServiceResult<ArticleDto>
                    {
                        IsSuccess = true,
                        Data = null,
                        Error = null
                    };
                }

                var articleModel = _mapper.Map<ArticleDto>(article);
                var author = await _users.GetFirstWhere(
                    u => u.Id == article.AuthorId,
                    u => u.UpdatedOn
                );
                articleModel.AuthorName = author.UserName;

                articleModel.Tags = new List<string>();
                var articleTags = await _articleTags.GetAllWhere(
                    t => t.ArticleId == id,
                    t => t.CreatedOn
                );

                if (articleTags.Count > 0)
                {
                    var tagIds = articleTags.Select(at => at.TagId).ToList();
                    var tags = await _tags.GetAllWhere(
                        tag => tagIds.Contains(tag.Id),
                        tag => tag.CreatedOn
                    );
                    articleModel.Tags = tags.Select(t => t.Name).ToList();
                }

                _logger.LogDebug($"Returning article with id: {id}");
                return new ServiceResult<ArticleDto>
                {
                    IsSuccess = true,
                    Data = articleModel,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch article with id: {id}");
                return new ServiceResult<ArticleDto>
                {
                    IsSuccess = false,
                    Data = null,
                    Error = new ServiceError { StackTrace = ex.StackTrace, Message = ex.Message }
                };
            }
        }

        public async Task<ServiceResult<ArticleDto>> Update(Guid articleId, ArticleDto articleDto)
        {
            try
            {
                var articleToUpdate = await _articles.GetById(articleId);
                articleToUpdate.Title = articleDto.Title;
                articleToUpdate.Content = articleDto.Content;

                if (articleToUpdate.ArticleTags != null)
                {
                    _logger.LogDebug($"Deleting existing ArticleTags for Article with id: {articleId}");
                    foreach (var articleTagEntity in articleToUpdate.ArticleTags)
                    {
                        await _articleTags.Delete(articleTagEntity.Id);
                    }
                }

                if (articleDto.Tags != null && articleDto.Tags.Count > 0)
                {
                    _logger.LogDebug($"Creating updated ArticleTags for Article with id: {articleId}");
                    foreach (var newTag in articleDto.Tags)
                    {
                        var newTagGuid = Guid.Parse(newTag);
                        var tag = await _tags.GetById(newTagGuid);

                        var newArticleTag = new ArticleTag
                        {
                            Id = newTagGuid,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedOn = DateTime.UtcNow,
                            Article = articleToUpdate,
                            Tag = tag
                        };
                        await _articleTags.Create(newArticleTag);
                    }
                }

                var updatedArticle = await _articles.Update(articleToUpdate);
                var articleResult = _mapper.Map<ArticleDto>(updatedArticle);

                _logger.LogDebug($"Updated article with id: {articleId}");

                return new ServiceResult<ArticleDto>
                {
                    IsSuccess = true,
                    Data = articleResult,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update article: {ex}");
                return new ServiceResult<ArticleDto>
                {
                    IsSuccess = false,
                    Data = null,
                    Error = new ServiceError { StackTrace = ex.StackTrace, Message = ex.Message }
                };
            }
        }

        public async Task<ServiceResult<Guid>> Create(ArticleDto articleDto)
        {
            try
            {
                var article = _mapper.Map<Article>(articleDto);
                var authorGuid = Guid.Parse(articleDto.AuthorId);
                var author = await _users.GetById(authorGuid);
                article.Author = author;
                var newArticleId = await _articles.Create(article);
                if (articleDto.Tags == null)
                {
                    _logger.LogDebug($"Returning new article with id: {newArticleId}");
                    return new ServiceResult<Guid>
                    {
                        IsSuccess = true,
                        Data = newArticleId,
                        Error = null
                    };
                }

                foreach (var articleTag in articleDto.Tags)
                {
                    var articleTagGuid = Guid.Parse(articleTag);
                    var foundArticleTag = await _tags.GetFirstWhere(
                        tag => tag.Id == articleTagGuid,
                        tag => tag.CreatedOn
                    );

                    if (foundArticleTag == null)
                        continue;

                    var newArticleTag = new ArticleTag
                    {
                        Id = Guid.NewGuid(),
                        Article = article,
                        Tag = foundArticleTag,
                    };
                    await _articleTags.Create(newArticleTag);
                }

                _logger.LogDebug($"Returning new article with id: {newArticleId}");
                return new ServiceResult<Guid>
                {
                    IsSuccess = true,
                    Data = newArticleId,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create article: {ex}");
                return new ServiceResult<Guid>
                {
                    IsSuccess = false,
                    Data = Guid.Empty,
                    Error = new ServiceError { StackTrace = ex.StackTrace, Message = ex.Message }
                };
            }
        }

        public async Task<ServiceResult<Guid>> Delete(Guid id)
        {
            try
            {
                await _articles.Delete(id);

                _logger.LogDebug($"Deleted article with id: {id}");

                return new ServiceResult<Guid>
                {
                    IsSuccess = true,
                    Data = id,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete article: {ex}");
                return new ServiceResult<Guid>
                {
                    IsSuccess = false,
                    Data = id,
                    Error = new ServiceError { StackTrace = ex.StackTrace, Message = ex.Message }
                };
            }
        }
    }
}
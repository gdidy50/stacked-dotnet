using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stacked.API.Models;
using Stacked.Models;
using Stacked.Services.Interfaces;

namespace Stacked.API.Controllers
{
    [ApiController]
    [Route("api/article")]
    public class ArticleController : ControllerBase
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleService _articleService;

        public ArticleController(
            ILogger<ArticleController> logger,
            IArticleService articleService)
        {
            _logger = logger;
            _articleService = articleService;
        }

        [HttpGet]
        public async Task<ActionResult> GetPaginatedArticles(
            [FromQuery] ManyArticlesRequest query)
        {
            var page = query.Page == 0 ? 1 : query.Page;
            var perPage = query.Page == 0 ? 3 : query.PerPage;
            var articles = await _articleService.GetAll(page, perPage);

            if (articles.Error != null)
            {
                _logger.LogError($"Error retrieving paginated articles: {articles.Error}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "There was an error retrieving Articles."
                );
            }

            _logger.LogDebug($"Retrieved Articles total: {articles.Data.TotalCount}");
            return Ok(articles.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetArticle(string id)
        {
            try
            {
                var guid = Guid.Parse(id);
                var article = await _articleService.GetById(guid);

                if (article.Data == null && article.Error == null)
                {
                    _logger.LogWarning($"Requested article not found: {id}");
                    return NotFound($"Article not found: {id}");
                }

                if (article.Error != null)
                {
                    _logger.LogError($"Error retrieving Article: {article.Error.Message}\n{article.Error.StackTrace}");
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        "There was an error retrieving Article."
                    );
                }

                _logger.LogDebug($"Retrieved Article: {id}");
                return Ok(article.Data);
            }
            catch (FormatException ex)
            {
                _logger.LogWarning($"There was a GUID format error for article {id}\n{ex.Message}\n{ex.StackTrace}");
                return BadRequest(id);
            }

        }

        [HttpPost]
        public async Task<ActionResult> CreateArticle([FromBody] ArticleDto article)
        {
            _logger.LogDebug($"Creating new Article: {article.Title}");
            var newArticle = await _articleService.Create(article);

            if (newArticle.Error != null)
            {
                _logger.LogError($"Error retrieving paginated articles: {newArticle.Error}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "There was an error creating the Article."
                );
            }

            _logger.LogDebug($"Created Article: {article.Title} ({newArticle.Data})");
            return StatusCode(
                    StatusCodes.Status201Created,
                    new { id = newArticle.Data }
                );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateArticle(string id, [FromBody] ArticleDto article)
        {
            try
            {
                var guid = Guid.Parse(id);
                var updatedArticle = await _articleService.Update(guid, article);
                if (updatedArticle.Error != null)
                {
                    _logger.LogError($"Error updating Article {id}: {updatedArticle.Error.Message}");
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        "There was an error updating the Article"
                    );
                }

                _logger.LogDebug($"Updated Article: {id}");
                return Ok(updatedArticle.Data.Id);
            }
            catch (FormatException ex)
            {
                _logger.LogWarning($"There was a GUID format error for article {id}\n{ex.Message}\n{ex.StackTrace}");
                return BadRequest(id);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteArticle(string id)
        {
            try
            {
                var guid = Guid.Parse(id);
                var deleted = await _articleService.Delete(guid);
                if (deleted.Error != null)
                {
                    _logger.LogError($"Error deleted Article {id}: {deleted.Error.Message}");
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        "There was an error deleted the Article"
                    );
                }

                _logger.LogDebug($"Updated Article: {id}");
                return Ok(deleted.Data);
            }
            catch (FormatException ex)
            {
                _logger.LogWarning($"There was a GUID format error for article {id}\n{ex.Message}\n{ex.StackTrace}");
                return BadRequest(id);
            }
        }
    }
}

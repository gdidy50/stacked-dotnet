using Xunit;
using Moq;
using Stacked.Data;
using Stacked.Data.Models;
using Stacked.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Stacked.Tests.Helpers;

namespace Stacked.Tests
{
    public class ArticleServiceTests
    {
        [Fact(DisplayName = "Invokes the article repository once to return a PagedServiceResult when GetAll is called")]
        public void Invokes_article_repository_to_return_paged_service_result_for_GetAll()
        {
            var articleRepoMock = new Mock<IRepository<Article>>();
            var articleTagRepoMock = new Mock<IRepository<ArticleTag>>();
            var userRepoMock = new Mock<IRepository<User>>();
            var tagRepoMock = new Mock<IRepository<Tag>>();
            var logMock = new Mock<ILogger<ArticleService>>();
            var mapperMock = new Mock<IMapper>();

            var fakeArticles = FakeEntityFactory.GenerateFakeArticles(3);

            const int perPage = 20;
            const int pageNum = 1;

            articleRepoMock.Setup(s => s.GetAll(
                It.IsAny<int>(),
                It.IsAny<int>())
            ).ReturnsAsync(new Models.PaginationResult<Article>
            {
                PageNumber = pageNum,
                ResultsPerPage = perPage,
                Results = fakeArticles
            });

            var sut = new ArticleService(
                articleRepoMock.Object,
                articleTagRepoMock.Object,
                tagRepoMock.Object,
                userRepoMock.Object,
                logMock.Object,
                mapperMock.Object
            );
        }
    }
}

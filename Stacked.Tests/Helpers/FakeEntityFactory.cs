using System.Collections.Generic;
using Stacked.Data.Models;

namespace Stacked.Tests.Helpers
{
    public static class FakeEntityFactory
    {
        public static Article GenerateFakeArticle()
        {
            return new Article();
        }

        public static List<Article> GenerateFakeArticles(int n)
        {
            return new List<Article>();
        }
    }
}
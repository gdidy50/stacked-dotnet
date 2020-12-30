using System;

namespace Stacked.Data.Models
{
    public class Comment : EntityModel
    {
        public string CommenterName { get; set; }
        public string Message { get; set; }
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
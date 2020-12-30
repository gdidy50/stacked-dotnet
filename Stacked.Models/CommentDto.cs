using System;

namespace Stacked.Models
{
    public class CommentDto : BlogDto
    {
        public string CommenterName { get; set; }
        public string Message { get; set; }
        public Guid ArticleId { get; set; }
        public ArticleDto Article { get; set; }
    }
}
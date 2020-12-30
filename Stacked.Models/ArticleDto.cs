using System;
using System.Collections.Generic;

namespace Stacked.Models
{
    public class ArticleDto : BlogDto
    {
        public string Title { get; set; }
        public bool IsPublished { get; set; }
        public string Content { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public List<CommentDto> Comments { get; set; }
        public List<string> Tags { get; set; }
    }
}
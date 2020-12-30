using System;

namespace Stacked.Data.Models
{
    public class ArticleTag : EntityModel
    {
        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
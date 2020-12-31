namespace Stacked.API.Models
{
    public class ManyCommentsRequest
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public string ArticleId { get; set; }
    }
}
using System.Collections.Generic;

namespace Stacked.API.Models
{
    public class ManyArticlesRequest
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public List<string> Tags { get; set; }
    }
}
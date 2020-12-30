using System.Collections.Generic;

namespace Stacked.Data.Models
{
    public class User : EntityModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PictureUrl { get; set; }
        public IEnumerable<Article> Articles { get; set; }
    }
}
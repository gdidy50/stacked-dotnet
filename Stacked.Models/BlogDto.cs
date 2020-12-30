using System;

namespace Stacked.Models
{
    public abstract class BlogDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
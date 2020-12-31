using FluentValidation;
using Stacked.Models;

namespace Stacked.API.Validators
{
    public class ArticleValidator : AbstractValidator<ArticleDto>
    {
        public ArticleValidator()
        {
            RuleFor(x => x.Title).Length(1, 128);
            RuleFor(x => x.Content).Length(1, 1_000_000);
            RuleFor(x => x.IsPublished).NotNull();
        }
    }
}
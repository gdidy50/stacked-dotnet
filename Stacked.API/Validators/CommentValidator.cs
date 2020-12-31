using FluentValidation;
using Stacked.Models;

namespace Stacked.API.Validators
{
    public class CommentValidator : AbstractValidator<CommentDto>
    {
        public CommentValidator()
        {
            RuleFor(x => x.CommenterName).Length(1, 32);
            RuleFor(x => x.Message).Length(1, 1000);
            RuleFor(x => x.ArticleId).NotNull();
        }
    }
}
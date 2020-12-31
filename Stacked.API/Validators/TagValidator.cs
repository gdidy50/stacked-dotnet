using FluentValidation;
using Stacked.Models;

namespace Stacked.API.Validators
{
    public class TagValidator : AbstractValidator<TagDto>
    {
        public TagValidator()
        {
            RuleFor(x => x.Name).Length(1, 32);
        }
    }
}
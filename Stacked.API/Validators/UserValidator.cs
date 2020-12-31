using FluentValidation;
using Stacked.Models;

namespace Stacked.API.Validators
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.FirstName).Length(1, 128);
            RuleFor(x => x.LastName).Length(1, 128);
            RuleFor(x => x.UserName).Length(1, 32);
            RuleFor(x => x.Email).EmailAddress().NotNull();
            RuleFor(x => x.PictureUrl).Length(1, 512);
        }
    }
}
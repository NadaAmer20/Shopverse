using FluentValidation;
namespace Shopverse.Application.Features.Identity.Commands.LoginUser
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Request.UsernameOrEmail)
                .NotEmpty()
                    .WithMessage("Username or Email is required.")
                .MaximumLength(256)
                    .WithMessage("Username or Email must not exceed 256 characters.");

            RuleFor(x => x.Request.Password)
                .NotEmpty()
                    .WithMessage("Password is required.")
                .MinimumLength(6)
                    .WithMessage("Password must be at least 6 characters long.");
        }
    }
}

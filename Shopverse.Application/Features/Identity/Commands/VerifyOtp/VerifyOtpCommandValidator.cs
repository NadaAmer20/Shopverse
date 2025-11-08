using FluentValidation;

namespace Shopverse.Application.Features.Identity.Commands.VerifyOtp
{
    public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
    {
        public VerifyOtpCommandValidator()
        {
            RuleFor(x => x.Request.UsernameOrEmail)
                .NotEmpty()
                .WithMessage("Username or email is required.");

            RuleFor(x => x.Request.Code)
                .NotEmpty()
                .WithMessage("OTP code is required.")
                .Length(6)
                .WithMessage("OTP code must be 6 characters long.");
        }
    }
}

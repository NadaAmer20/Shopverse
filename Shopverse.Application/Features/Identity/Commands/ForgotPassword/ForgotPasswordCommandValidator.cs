using FluentValidation;
using Microsoft.AspNetCore.Http;
using Shopverse.Application.Features.Identity.Commands.ForgotPassword;
using System.Threading;

namespace Shopverse.Application.Features.Identity.Commands.ForgotPassword
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {

            RuleFor(x => x.Request.Email)
                .NotEmpty()
                .WithMessage("Email is required.") 
                .EmailAddress()
                .WithMessage("Invalid email format."); 
        }
    }
}

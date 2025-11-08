using FluentValidation;

namespace Shopverse.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Role name is required.");

            RuleFor(x => x.Request.Key)
                .NotEmpty().WithMessage("Role key is required.");
        }
    }
}

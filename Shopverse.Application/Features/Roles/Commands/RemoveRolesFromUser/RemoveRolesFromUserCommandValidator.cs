using FluentValidation;

namespace Shopverse.Application.Features.Roles.Commands.RemoveRolesFromUser
{
    public class RemoveRolesFromUserCommandValidator : AbstractValidator<RemoveRolesFromUserCommand>
    {
        public RemoveRolesFromUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.RoleIds)
                .NotNull().WithMessage("RoleIds are required.")
                .Must(ids => ids.Any()).WithMessage("At least one role must be provided.");
        }
    }
}

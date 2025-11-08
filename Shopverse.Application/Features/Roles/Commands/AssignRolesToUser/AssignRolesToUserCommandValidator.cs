using FluentValidation;
using System.Linq;

namespace Shopverse.Application.Features.Roles.Commands.AssignRolesToUser
{
    public class AssignRolesToUserCommandValidator : AbstractValidator<AssignRolesToUserCommand>
    {
        public AssignRolesToUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.RoleIds)
                .NotNull().WithMessage("RoleIds are required.")
                .Must(ids => ids.Any()).WithMessage("At least one role must be assigned.");
        }
    }
}

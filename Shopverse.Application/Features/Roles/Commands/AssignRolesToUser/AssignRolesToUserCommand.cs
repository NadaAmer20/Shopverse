using MediatR;
using Shopverse.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Roles.Commands.AssignRolesToUser
{
    public record AssignRolesToUserCommand(Guid UserId, List<Guid> RoleIds) : IRequest<ApiResponse<string>>;

}

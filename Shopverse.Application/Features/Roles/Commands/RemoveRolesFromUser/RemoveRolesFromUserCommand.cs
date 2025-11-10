using MediatR;
using Shopverse.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Roles.Commands.RemoveRolesFromUser
{
    public record RemoveRolesFromUserCommand(Guid UserId, Guid RoleId) : IRequest<ApiResponse<string>>;
}

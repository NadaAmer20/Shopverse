using MediatR;
using Shopverse.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Roles.Commands.DeleteRole
{
    public record DeleteRoleCommand(Guid RoleId) : IRequest<ApiResponse<string>>;

}

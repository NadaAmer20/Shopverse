using MediatR;
using PNUStudentPortal.Application.DTOs.Roles;
using Shopverse.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Roles.Queries.GetRoleById
{
    public record GetRoleByIdQuery(Guid RoleId) : IRequest<ApiResponse<RoleDetailsResponse>>;

}

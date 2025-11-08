using MediatR;
using PNUStudentPortal.Application.DTOs.Roles;
using Shopverse.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Roles.Queries.GetRoles
{
    public record GetRolesQuery(
          int Page = 1,
          int PageSize = 0,
          string? Search = null,
          bool IsPaginated = true
      ) : IRequest<ApiResponse<List<RoleResponse>>>;
}

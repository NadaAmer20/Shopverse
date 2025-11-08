using MediatR;
using Shopverse.Application.DTOs.User;
using Shopverse.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Identity.Queries.GetUsers
{
    public record GetUsersQuery(int Page = 1, int PageSize = 10, bool IsPaginated = true)
        : IRequest<ApiResponse<List<UserDto>>>;

}

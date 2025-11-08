using MediatR;
using PNUStudentPortal.Application.DTOs.Roles;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Roles.Queries.GetRoles
{
    public class GetRolesHandler : IRequestHandler<GetRolesQuery, ApiResponse<List<RoleResponse>>>
    {
        private readonly IRepository<Role> _roleRepo;

        public GetRolesHandler(IRepository<Role> roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<ApiResponse<List<RoleResponse>>> Handle(GetRolesQuery request, CancellationToken ct)
        {
            var query = _roleRepo.GetQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(r => r.Name.Contains(request.Search) || r.Key.Contains(request.Search));

            var total = query.Count();

            List<RoleResponse> items;
            if (request.IsPaginated)
            {
                items = query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(r => new RoleResponse(r.Id, r.Name, r.Key, r.Description))
                    .ToList();
            }
            else
            {
                items = query
                    .Select(r => new RoleResponse(r.Id, r.Name, r.Key, r.Description))
                    .ToList();
            }

            var pagination = new Pagination
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = total,
                CurrentRecords = items.Count
            };

            return ApiResponse<List<RoleResponse>>.Success(items, "Roles retrieved successfully.", pagination);
        }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using PNUStudentPortal.Application.DTOs.Roles;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Roles.Queries.GetRoleById
{
    public class GetRoleByIdHandler : IRequestHandler<GetRoleByIdQuery, ApiResponse<RoleDetailsResponse>>
    {
        private readonly IRepository<Role> _roleRepo;

        public GetRoleByIdHandler(IRepository<Role> roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<ApiResponse<RoleDetailsResponse>> Handle(GetRoleByIdQuery request, CancellationToken ct)
        {
            var role = await _roleRepo.GetByIdAsync(request.RoleId);

            if (role == null)
            {
                return ApiResponse<RoleDetailsResponse>.Fail("Role not found.");
            }

            var response = new RoleDetailsResponse(
                role.Id,
                role.Name,
                role.Key,
                role.Description
            );

            return ApiResponse<RoleDetailsResponse>.Success(response, "Role fetched successfully.");
        }
    }
}

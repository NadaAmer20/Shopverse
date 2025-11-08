using MediatR;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Roles.Commands.UpdateRole
{
    public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, ApiResponse<string>>
    {
        private readonly IRepository<Role> _roleRepo;

        public UpdateRoleHandler(IRepository<Role> roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<ApiResponse<string>> Handle(UpdateRoleCommand request, CancellationToken ct)
        {
            var role = await _roleRepo.GetByIdAsync(request.RoleId);
            if (role == null)
            {
                return ApiResponse<string>.Fail("Role not found.");
            }

            role.Name = request.Request.Name;
            role.Description = request.Request.Description;
            role.UpdatedAt = DateTime.UtcNow;

            await _roleRepo.UpdateAsync(role, ct);

            return ApiResponse<string>.Success("Role updated successfully.");
        }
    }
}

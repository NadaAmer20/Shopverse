using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Roles.Commands.DeleteRole
{
    public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, ApiResponse<string>>
    {
        private readonly IRepository<Role> _roleRepo;

        public DeleteRoleHandler(IRepository<Role> roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<ApiResponse<string>> Handle(DeleteRoleCommand request, CancellationToken ct)
        {
            var role = await _roleRepo.GetQueryable()
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.Id == request.RoleId, ct);

            if (role == null)
            {
                return ApiResponse<string>.Fail("Role not found.");
            }

            role.UserRoles.Clear();

            await _roleRepo.RemoveAsync(role, ct);

            return ApiResponse<string>.Success("Role deleted successfully.");
        }
    }
}

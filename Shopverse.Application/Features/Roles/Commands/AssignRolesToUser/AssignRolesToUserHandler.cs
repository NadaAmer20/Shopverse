using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Roles.Commands.AssignRolesToUser
{
    public class AssignRolesToUserHandler : IRequestHandler<AssignRolesToUserCommand, ApiResponse<string>>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Role> _roleRepo;

        public AssignRolesToUserHandler(IRepository<User> userRepo, IRepository<Role> roleRepo)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
        }

        public async Task<ApiResponse<string>> Handle(AssignRolesToUserCommand request, CancellationToken ct)
        {
            // البحث عن المستخدم
            var user = await _userRepo.GetQueryable()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

            if (user == null)
                return ApiResponse<string>.Fail("User not found.");

            var role = await _roleRepo.GetQueryable()
                .FirstOrDefaultAsync(r => r.Id == request.RoleId, ct);   

            if (role == null)
                return ApiResponse<string>.Fail("Role not found.");

            if (user.RoleId == role.Id)
                return ApiResponse<string>.Fail("User already has this role.");

            user.RoleId = role.Id;
            user.Role = role;

            await _userRepo.UpdateAsync(user, ct);

            return ApiResponse<string>.Success("Role assigned to user successfully.");
        }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System;
using System.Collections.Generic;
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
            var user = await _userRepo.GetQueryable()
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

            if (user == null)
                return ApiResponse<string>.Fail("User not found.");

            var roles = await _roleRepo.GetAllAsync(r => request.RoleIds.Contains(r.Id), ct);

            var toRemove = user.UserRoles
                .Where(ur => !request.RoleIds.Contains(ur.RoleId))
                .ToList();

            foreach (var ur in toRemove)
                user.UserRoles.Remove(ur);

            foreach (var role in roles)
            {
                if (!user.UserRoles.Any(ur => ur.RoleId == role.Id))
                    user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
            }

            await _userRepo.UpdateAsync(user, ct);

            return ApiResponse<string>.Success("Roles assigned to user successfully.");
        }
    }
}

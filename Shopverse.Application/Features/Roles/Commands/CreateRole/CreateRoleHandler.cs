using MediatR;
using Shopverse.Application.Features.Roles.Commands.CreateRole;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, ApiResponse<Guid>>
    {
        private readonly IRepository<Role> _roleRepo;

        public CreateRoleHandler(IRepository<Role> roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateRoleCommand request, CancellationToken ct)
        {
            var exists = await _roleRepo.IsExistAsync(r => r.Key == request.Request.Key, ct);
            if (exists)
            {
                return ApiResponse<Guid>.Fail("Role key already exists.");
            }

            var role = new Role
            {
                Name = request.Request.Name,
                Key = request.Request.Key,
                Description = request.Request.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _roleRepo.AddAsync(role, ct);

            return ApiResponse<Guid>.Success(role.Id, "Role created successfully.");
        }
    }
}

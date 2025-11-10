using MediatR;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Roles.Commands.RemoveRolesFromUser
{
    public class RemoveRolesFromUserHandler : IRequestHandler<RemoveRolesFromUserCommand, ApiResponse<string>>
    {
        private readonly IRepository<User> _userRepo;

        public RemoveRolesFromUserHandler(IRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ApiResponse<string>> Handle(RemoveRolesFromUserCommand request, CancellationToken ct)
        {
            var user = await _userRepo.GetByIdAsync(request.UserId);
            if (user == null)
                return ApiResponse<string>.Fail("User not found.");

            if (user.RoleId != request.RoleId)
                return ApiResponse<string>.Fail("Role not assigned to user.");

            user.RoleId = Guid.Empty;   
            user.Role = null;   

            await _userRepo.UpdateAsync(user, ct);

            return ApiResponse<string>.Success("Role removed from user successfully.");
        }
    }
}

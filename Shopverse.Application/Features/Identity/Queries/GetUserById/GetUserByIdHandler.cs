using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shopverse.Application.DTOs.User;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Identity.Queries.GetUserById
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, ApiResponse<UserDto?>>
    {
        private readonly IRepository<User> _userRepo;

        public GetUserByIdHandler(IRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ApiResponse<UserDto?>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var userQuery = _userRepo.GetQueryable()
                                     .Where(u => u.Id == request.UserId);

            var user = await userQuery.FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return ApiResponse<UserDto?>.Fail("User not found.");

            var dto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = user.Role != null ? new RoleDto
                {
                    Id = user.Role.Id,
                    Name = user.Role.Name
                } : null  
            };

            return ApiResponse<UserDto?>.Success(dto, "User fetched successfully.");
        }

    }
}

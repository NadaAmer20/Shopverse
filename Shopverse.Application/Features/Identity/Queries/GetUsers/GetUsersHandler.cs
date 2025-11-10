using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopverse.Application.DTOs.User;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Identity.Queries.GetUsers
{
    public class GetUsersHandler : IRequestHandler<GetUsersQuery, ApiResponse<List<UserDto>>>
    {
        private readonly IRepository<User> _userRepo;

        public GetUsersHandler(IRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ApiResponse<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _userRepo.GetQueryable()
                .Include(ur => ur.Role);

            var allUsers = await query.ToListAsync(cancellationToken);
            List<User> users;
            Pagination? pagination = null;

            if (request.IsPaginated)
            {
                var skip = (request.Page - 1) * request.PageSize;
                users = allUsers.Skip(skip).Take(request.PageSize).ToList();

                pagination = new Pagination
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = allUsers.Count,
                    CurrentRecords = users.Count
                };
            }
            else
            {
                users = allUsers;
            }

            var dtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                IsActive = u.IsActive,
                Role = u.Role != null ? new RoleDto
                {
                    Id = u.Role.Id,
                    Name = u.Role.Name
                } : null
            }).ToList();

            return ApiResponse<List<UserDto>>.Success(dtos, "Users fetched successfully", pagination);
        }
    }
}

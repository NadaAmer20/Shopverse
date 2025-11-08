using MediatR;
using Microsoft.AspNetCore.Http;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Identity.Commands.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, ApiResponse<Guid>>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterUserHandler(
            IRepository<User> userRepo,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepo = userRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var req = request.Request;

            var exists = await _userRepo.IsExistAsync(
                u => u.Email == req.Email || u.Username == req.Username,
                cancellationToken);

            if (exists)
            {
                return ApiResponse<Guid>.Fail("User already exists.");
            }

            var user = new User
            {
                Name = req.Name,
                Username = req.Username,
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user, cancellationToken);

            return ApiResponse<Guid>.Success(user.Id, "User registered successfully.");
        }


    }
}

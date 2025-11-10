using MediatR;
using Microsoft.EntityFrameworkCore;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Interfaces.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Identity.Commands.ActivateUser
{
    public class ActivateUserHandler : IRequestHandler<ActivateUserCommand, ApiResponse<string>>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IJwtService _jwtService;

        public ActivateUserHandler(IRepository<User> userRepo, IJwtService jwtService)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<string>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            var email = _jwtService.GetEmailFromToken(request.Token);
            if (string.IsNullOrEmpty(email))
            {
                return ApiResponse<string>.Fail("Invalid or expired token.");
            }

            var user = await _userRepo.GetQueryable().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return ApiResponse<string>.Fail("User not found.");
            }

            user.IsActive = true;
            await _userRepo.UpdateAsync(user, cancellationToken);

            return ApiResponse<string>.Success("User account activated successfully.");
        }
    }

}

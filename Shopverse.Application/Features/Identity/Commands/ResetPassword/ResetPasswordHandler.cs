using MediatR;
using Shopverse.Application.Features.Identity.Commands.ResetPassword;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Enums;
using Shopverse.Domain.Interfaces.OTP;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Identity.Commands.ResetPassword
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, ApiResponse<string>>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IOtpService _otpService;

        public ResetPasswordHandler(
            IRepository<User> userRepo,
            IOtpService otpService)
        {
            _userRepo = userRepo;
            _otpService = otpService;
        }

        public async Task<ApiResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var users = await _userRepo.GetListAsync(u => u.Email == request.Request.Email, cancellationToken: cancellationToken);
            var user = users.FirstOrDefault();
            if (user == null)
            {
                return ApiResponse<string>.Fail("Invalid reset data: user not found.");
            }

            var isOtpValid = await _otpService.ValidateOtpAsync(user, request.Request.Code, OtpPurpose.ResetPassword, cancellationToken);
            if (!isOtpValid)
            {
                return ApiResponse<string>.Fail("Invalid or expired verification code.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.NewPassword);
            await _userRepo.UpdateAsync(user, cancellationToken);

            return ApiResponse<string>.Success("Password reset successfully.");
        }
    }
}

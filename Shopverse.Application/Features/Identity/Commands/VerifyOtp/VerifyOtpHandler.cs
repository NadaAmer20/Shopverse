using MediatR;
using Microsoft.AspNetCore.Http;
using Shopverse.Application.DTOs.User;
using Shopverse.Application.Features.Identity.Commands.VerifyOtp;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Enums;
using Shopverse.Domain.Interfaces.OTP;
using Shopverse.Domain.Interfaces.Security;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Identity.Commands.VerifyOtp
{
    public class VerifyOtpHandler : IRequestHandler<VerifyOtpCommand, ApiResponse<AuthResult>>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IOtpService _otpService;
        private readonly IJwtService _jwtService;

        public VerifyOtpHandler(
            IRepository<User> userRepo,
            IOtpService otpService,
            IJwtService jwtService)
        {
            _userRepo = userRepo;
            _otpService = otpService;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<AuthResult>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var q = request.Request.UsernameOrEmail?.Trim() ?? "";

            var user = (await _userRepo.GetListAsync(u => u.Email == q || u.Username == q, cancellationToken: cancellationToken))
                .FirstOrDefault();
            if (user == null)
            {
                return ApiResponse<AuthResult>.Fail("User not found.");
            }

            var isValidOtp = await _otpService.ValidateOtpAsync(user, request.Request.Code, OtpPurpose.Login, cancellationToken);
            if (!isValidOtp)
            {
                return ApiResponse<AuthResult>.Fail("Invalid or expired OTP.");
            }

            var authTokenResult = await _jwtService.GenerateToken(user.Id, user.Username, user.Email);
            var authResultDto = new AuthResult(authTokenResult.Token, authTokenResult.ExpiryDate);

            return ApiResponse<AuthResult>.Success(authResultDto, "OTP verified successfully.");
        }
    }
}

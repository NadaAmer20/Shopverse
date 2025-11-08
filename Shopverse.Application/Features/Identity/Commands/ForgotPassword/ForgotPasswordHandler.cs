using MediatR;
using Microsoft.AspNetCore.Http;
using Shopverse.Application.Responses;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Enums;
using Shopverse.Domain.Interfaces.OTP;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Shopverse.Application.Features.Identity.Commands.ForgotPassword
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ApiResponse<string>>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IOtpService _otpService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForgotPasswordHandler(
            IRepository<User> userRepo,
            IOtpService otpService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepo = userRepo;
            _otpService = otpService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = (await _userRepo.GetListAsync(u => u.Email == request.Request.Email, cancellationToken: cancellationToken))
                       .FirstOrDefault();

            if (user == null)
            {
                return ApiResponse<string>.Fail("User not found with the provided email.");
            }

            await _otpService.GenerateAndSendOtpAsync(user, OtpPurpose.ResetPassword, cancellationToken);

            return ApiResponse<string>.Success("OTP has been sent successfully to your email.");
        }
    }
}

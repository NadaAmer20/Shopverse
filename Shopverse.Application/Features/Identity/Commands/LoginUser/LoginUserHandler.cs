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
namespace Shopverse.Application.Features.Identity.Commands.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, ApiResponse<bool>>
    {
        private readonly IRepository<User> _userRepo;
        private readonly IOtpService _otpService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginUserHandler(
            IRepository<User> userRepo,
            IOtpService otpService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepo = userRepo;
            _otpService = otpService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<bool>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var q = request.Request.UsernameOrEmail?.Trim() ?? "";

            var users = await _userRepo.GetListAsync(u => u.Email == q || u.Username == q, cancellationToken: cancellationToken);
            var user = users.FirstOrDefault();

            if (user == null)
                return ApiResponse<bool>.Fail("Invalid username or password.");

            if (!BCrypt.Net.BCrypt.Verify(request.Request.Password, user.PasswordHash))
                return ApiResponse<bool>.Fail("Invalid username or password.");


            if (!user.IsActive)
                return ApiResponse<bool>.Fail("Account is disabled.");

            await _otpService.GenerateAndSendOtpAsync(user, OtpPurpose.Login, cancellationToken);

            return ApiResponse<bool>.Success(true, "OTP sent successfully.");
        }
    }
}

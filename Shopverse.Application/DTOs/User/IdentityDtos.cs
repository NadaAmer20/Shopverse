using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.DTOs.User
{

    public record RegisterRequest(string Name, string Username, string Email, string Password);
    public record LoginRequest(string UsernameOrEmail, string Password);
    public record VerifyOtpRequest(string UsernameOrEmail, string Code);
    public record ForgotPasswordRequest(string Email);
    public record ResetPasswordRequest(string Email, string Code, string NewPassword);

    public record AuthResult(string Token, DateTime ExpiresAt);
}

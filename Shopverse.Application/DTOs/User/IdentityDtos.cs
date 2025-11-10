using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.DTOs.User
{

    public class RegisterUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;  
        public Guid RoleId { get; set; } 
    }
    public record LoginRequest(string UsernameOrEmail, string Password);
    public record VerifyOtpRequest(string UsernameOrEmail, string Code);
    public record ForgotPasswordRequest(string Email);
    public record ResetPasswordRequest(string Email, string Code, string NewPassword);

    public record AuthResult(string Token, DateTime ExpiresAt);
}

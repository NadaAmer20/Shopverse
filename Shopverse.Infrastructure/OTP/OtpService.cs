using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Enums;
using Shopverse.Domain.Interfaces.Communication;
using Shopverse.Domain.Interfaces.OTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNUStudentPortal.Infrastructure.Services.OTP
{
    public class OtpService : IOtpService
    {
        private readonly IRepository<OtpVerification> _otpRepo;
        private readonly IEmailService _emailService;

        public OtpService(IRepository<OtpVerification> otpRepo, IEmailService emailService)
        {
            _otpRepo = otpRepo;
            _emailService = emailService;
        }

        public async Task<string> GenerateAndSendOtpAsync(User user, OtpPurpose purpose = OtpPurpose.Login, CancellationToken cancellationToken = default)
        {
            var code = new Random().Next(100000, 999999).ToString();
            var expiryMinutes = purpose == OtpPurpose.Login ? 10 : 15;

            var otp = new OtpVerification
            {
                UserId = user.Id,
                Code = code,
                Expiry = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Purpose = purpose,
                IsUsed = false
            };

            await _otpRepo.AddAsync(otp, cancellationToken);

            var (subject, body) = GetEmailContent(purpose, code, expiryMinutes);
            await _emailService.SendEmailAsync(user.Email, subject, body, cancellationToken);

            return code;
        }
        public async Task<bool> ValidateOtpAsync(User user, string code, OtpPurpose purpose, CancellationToken cancellationToken)
        {
            var otps = await _otpRepo.GetListAsync(o =>
                o.UserId == user.Id &&
                o.Code == code &&
                !o.IsUsed &&
                o.Purpose == purpose);

            var otp = otps.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            if (otp == null || otp.Expiry < DateTime.UtcNow) return false;

            otp.IsUsed = true;
            await _otpRepo.UpdateAsync(otp, cancellationToken);
            return true;
        }
        private (string subject, string body) GetEmailContent(OtpPurpose purpose, string code, int expiryMinutes)
        {
            return purpose == OtpPurpose.Login
                ? ("Your login OTP", $"Your login code: <b>{code}</b>. It expires in {expiryMinutes} minutes.")
                : ("Password reset code", $"Your password reset code is <b>{code}</b>. It expires in {expiryMinutes} minutes.");
        }

    }

}

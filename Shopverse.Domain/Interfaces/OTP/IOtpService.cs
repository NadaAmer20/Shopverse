using Shopverse.Domain.Entities;
using Shopverse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Domain.Interfaces.OTP
{
    public interface IOtpService
    {
        Task<string> GenerateAndSendOtpAsync(User user, OtpPurpose purpose, CancellationToken cancellationToken);
        Task<bool> ValidateOtpAsync(User user, string code, OtpPurpose purpose, CancellationToken cancellationToken);

    }

}

using Shopverse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Domain.Entities
{
    public class OtpVerification : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public bool IsUsed { get; set; } = false;
        public OtpPurpose Purpose { get; set; }
    }
}

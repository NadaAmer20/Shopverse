using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Domain.Enums
{
    public enum EntityTypeEnum
    {
        Category = 1,
        Newsletter = 2,
        Role = 3,
        Permission = 4,
        User = 5,
        Favicon = 6,
        Logo = 7,
        Setting = 8,
    }
    public enum OtpPurpose
    {
        Login,
        ResetPassword
    }

}

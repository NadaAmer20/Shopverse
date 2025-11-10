using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Domain.Results
{
    public record AuthTokenResult(string Token, DateTime ExpiryDate , string newRefreshToken);

}

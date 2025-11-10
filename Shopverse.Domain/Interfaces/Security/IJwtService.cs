using Shopverse.Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Domain.Interfaces.Security
{
    public interface IJwtService
    {
        Task<AuthTokenResult> GenerateToken(Guid userId, string username, string email);
        string GetEmailFromToken(string token);
    }
}

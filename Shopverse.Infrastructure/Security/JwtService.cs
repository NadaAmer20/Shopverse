using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shopverse.Domain.Abstractions;
using Shopverse.Domain.Entities;
using Shopverse.Domain.Interfaces.Security;
using Shopverse.Domain.Results;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Infrastructure.Services.Security
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _cfg;
        private readonly IRepository<User> _userRepo;

        public JwtService(IConfiguration cfg, IRepository<User> userRepo)
        {
            _cfg = cfg;
            _userRepo = userRepo;
        }

        public async Task<AuthTokenResult> GenerateToken(Guid userId, string username, string email)
        {
            var jwt = _cfg.GetSection("Jwt");
            var secret = jwt.GetValue<string>("Secret")!;
            var issuer = jwt.GetValue<string>("Issuer");
            var audience = jwt.GetValue<string>("Audience");

            var user = await _userRepo.GetSingleAsync(
                u => u.Id == userId,
                include => include
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            );

            var roles = user?.UserRoles.Select(ur => ur.Role.Key).ToList() ?? new List<string>();

            var claims = new List<Claim>
            {
                new Claim("id", userId.ToString()),
                new Claim("username", username),
                new Claim("email", email)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthTokenResult(tokenStr, token.ValidTo);
        }
    }
}

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

            var user = await _userRepo.GetSingleAsync(u => u.Id == userId);

            var claims = new List<Claim>
            {
                new Claim("id", userId.ToString()),
                new Claim("username", username),
                new Claim("email", email)
            };

            claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));

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
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;   
            await _userRepo.UpdateAsync(user, CancellationToken.None);  

            return new AuthTokenResult(tokenStr, token.ValidTo, refreshToken);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<AuthTokenResult> RefreshToken(string refreshToken)
        {
            var user = await _userRepo.GetSingleAsync(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }


            // Now generate a new JWT
            var jwt = _cfg.GetSection("Jwt");
            var secret = jwt.GetValue<string>("Secret")!;
            var issuer = jwt.GetValue<string>("Issuer");
            var audience = jwt.GetValue<string>("Audience");

            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("email", user.Email)
            };

            claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));

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

             var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userRepo.UpdateAsync(user, CancellationToken.None);

            return new AuthTokenResult(tokenStr, token.ValidTo, newRefreshToken);
        }
        public string GetEmailFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var emailClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "email");

            return emailClaim?.Value;
        }
    }
}

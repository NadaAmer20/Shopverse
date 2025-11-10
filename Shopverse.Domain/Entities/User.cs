using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Guid? RoleId { get; set; }
        public Role? Role { get; set; } = null!;
        public string? Token { get; set; }  
        public string? RefreshToken { get; set; } 
    }

}

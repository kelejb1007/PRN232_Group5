using DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs.UserAccount
{
    public class AuthDTO
    {
        public class RegisterRequestDTO
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? FullName { get; set; }
            public String Role { get; set; } = string.Empty;
        }

        public class LoginRequestDTO
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class AuthResponseDTO
        {
            public string AccessToken { get; set; } = string.Empty;
            public DateTime ExpiresAtUtc { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }

}

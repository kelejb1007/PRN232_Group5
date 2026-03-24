using DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs.UserAccount
{
    public class AuthDTO
    {
        public class RegisterRequestDTO
        {
            [Required(ErrorMessage = "Username is required.")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Full Name is required.")]
            [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
            public string? FullName { get; set; }
            
            public String Role { get; set; } = string.Empty;
        }

        public class LoginRequestDTO
        {
            [Required(ErrorMessage = "Username is required.")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required.")]
            public string Password { get; set; } = string.Empty;
        }

        public class GoogleLoginRequestDTO
        {
            [Required]
            public string IdToken { get; set; } = string.Empty;
        }

        public class VerifyEmailRequestDTO
        {
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Token { get; set; } = string.Empty;
        }

        public class AuthResponseDTO
        {
            public string AccessToken { get; set; } = string.Empty;
            public DateTime ExpiresAtUtc { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }

        public class ForgotPasswordRequestDTO
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public class ResetPasswordRequestDTO
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Token { get; set; } = string.Empty;

            [Required]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
            public string NewPassword { get; set; } = string.Empty;
        }
        public class UserProfileDTO
        {
            public string Username { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? FullName { get; set; }
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string Role { get; set; } = string.Empty;
        }

        public class UpdateProfileRequestDTO
        {
            [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
            public string? FullName { get; set; }
            
            [Phone(ErrorMessage = "Invalid phone number.")]
            public string? Phone { get; set; }
            
            [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
            public string? Address { get; set; }
        }
    }
}

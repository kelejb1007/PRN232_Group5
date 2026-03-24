using DAL.DTOs.UserAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DAL.DTOs.UserAccount.AuthDTO;

namespace BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO request);
        Task<bool> RegisterAsync(RegisterRequestDTO request);
        Task<AuthResponseDTO> GoogleLoginAsync(GoogleLoginRequestDTO request);
        Task<bool> VerifyEmailAsync(VerifyEmailRequestDTO request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDTO request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO request);
        Task<bool> ResendOtpAsync(string email, string type);
        Task<UserProfileDTO?> GetProfileAsync(string username);
        Task<bool> UpdateProfileAsync(string username, UpdateProfileRequestDTO request);
    }
}

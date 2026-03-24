using static Models.UserAccount.AuthDTO;

namespace Intelligence_Book_WEB.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO request);
        Task<bool> RegisterAsync(RegisterRequestDTO request);
        Task<AuthResponseDTO?> GoogleLoginAsync(GoogleLoginRequestDTO request);
        Task<bool> VerifyEmailAsync(VerifyEmailRequestDTO request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDTO request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO request);
        Task<bool> ResendOtpAsync(string email, string type);
        Task LogoutAsync(string? accessToken);
    }
}

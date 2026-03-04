using static Models.UserAccount.AuthDTO;

namespace Intelligence_Book_WEB.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO request);
        Task<AuthResponseDTO?> RegisterAsync(RegisterRequestDTO request);
        Task LogoutAsync(string? accessToken);

    }
}

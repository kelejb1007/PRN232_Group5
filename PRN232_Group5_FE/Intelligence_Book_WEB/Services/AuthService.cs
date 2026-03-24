using Intelligence_Book_WEB.Services.Interfaces;
using System.Net;
using System.Net.Http.Json;
using static Models.UserAccount.AuthDTO;

namespace Intelligence_Book_WEB.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
        }

        public async Task<bool> RegisterAsync(RegisterRequestDTO request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<AuthResponseDTO?> GoogleLoginAsync(GoogleLoginRequestDTO request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/google-login", request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
        }

        public async Task<bool> VerifyEmailAsync(VerifyEmailRequestDTO request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/verify-email", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDTO request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/forgot-password", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/reset-password", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ResendOtpAsync(string email, string type)
        {
            var response = await _http.PostAsync($"api/auth/resend-otp?email={WebUtility.UrlEncode(email)}&type={WebUtility.UrlEncode(type)}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task LogoutAsync(string? accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }

            await _http.SendAsync(request);
        }
    }
}

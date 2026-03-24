using Intelligence_Book_WEB.Services.Interfaces;
using System.Net;
using System.Net.Http.Headers;
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

        public async Task<AuthResponseDTO?> RegisterAsync(RegisterRequestDTO request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
        }

        public async Task LogoutAsync(string? accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            await _http.SendAsync(request);
        }
    }

}

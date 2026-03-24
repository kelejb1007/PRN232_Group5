using Intelligence_Book_WEB.Services.User.Interfaces;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static Models.UserAccount.AuthDTO;
using System.Net.Http.Headers;

namespace Intelligence_Book_WEB.Services.User
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _http;

        public ProfileService(HttpClient http)
        {
            _http = http;
        }

        public async Task<UserProfileDTO?> GetProfileAsync(string? accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/profile");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserProfileDTO>();
            }
            return null;
        }

        public async Task<bool> UpdateProfileAsync(string? accessToken, UpdateProfileRequestDTO updateRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "api/auth/profile");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            request.Content = JsonContent.Create(updateRequest);

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}

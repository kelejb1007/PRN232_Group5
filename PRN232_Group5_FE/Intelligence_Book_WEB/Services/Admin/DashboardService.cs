using Intelligence_Book_WEB.Models.Dashboard;
using Intelligence_Book_WEB.Services.Admin.Interfaces;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Intelligence_Book_WEB.Services.Admin
{
    public class DashboardService : IDashboardService
    {
        private readonly HttpClient _http;

        public DashboardService(HttpClient http)
        {
            _http = http;
        }

        public async Task<DashboardSummaryDTO?> GetDashboardSummaryAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/admin/Dashboard/summary");
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DashboardSummaryDTO>();
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"API returned {response.StatusCode}: {errorContent}");
        }
    }
}

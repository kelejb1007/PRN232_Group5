using Intelligence_Book_WEB.Models.Order;
using Intelligence_Book_WEB.Services.User.Interfaces;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Intelligence_Book_WEB.Services.User
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _http;

        public OrderService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<OrderHistoryDTO>> GetOrderHistoryAsync(string? accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Order/history");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<OrderHistoryDTO>>() ?? Enumerable.Empty<OrderHistoryDTO>();
            }
            return Enumerable.Empty<OrderHistoryDTO>();
        }

        public async Task<OrderHistoryDTO?> GetOrderDetailsAsync(string? accessToken, int orderId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Order/{orderId}");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OrderHistoryDTO>();
            }
            return null;
        }
    }
}

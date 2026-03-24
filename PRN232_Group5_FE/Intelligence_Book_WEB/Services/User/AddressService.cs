using Intelligence_Book_WEB.Models.DeliveryAddress;
using Intelligence_Book_WEB.Services.User.Interfaces;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Intelligence_Book_WEB.Services.User
{
    public class AddressService : IAddressService
    {
        private readonly HttpClient _http;

        public AddressService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<DeliveryAddressDTO>> GetMyAddressesAsync(string? accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Address");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<DeliveryAddressDTO>>() ?? Enumerable.Empty<DeliveryAddressDTO>();
            }
            return Enumerable.Empty<DeliveryAddressDTO>();
        }

        public async Task<bool> AddAddressAsync(string? accessToken, DeliveryAddressDTO addressRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Address");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            request.Content = JsonContent.Create(addressRequest);

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAddressAsync(string? accessToken, int addressId, DeliveryAddressDTO addressRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/Address/{addressId}");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            request.Content = JsonContent.Create(addressRequest);

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAddressAsync(string? accessToken, int addressId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/Address/{addressId}");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SetDefaultAddressAsync(string? accessToken, int addressId)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/Address/{addressId}/default");
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}

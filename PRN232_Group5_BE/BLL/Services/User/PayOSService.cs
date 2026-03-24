using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Intelligence_Book_API.Services.User
{
    public class PayOSService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _configuration;

        public PayOSService(HttpClient http, IConfiguration configuration)
        {
            _http = http;
            _configuration = configuration;
        }

        public async Task<string> CreatePayment(int orderId, decimal amount)
        {
            var clientId = _configuration["PayOS:ClientId"];
            var apiKey = _configuration["PayOS:ApiKey"];
            var checksumKey = _configuration["PayOS:ChecksumKey"];

            if (string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(checksumKey))
            {
                throw new Exception("Thiếu cấu hình PayOS trong appsettings.json");
            }

            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int finalAmount = (int)amount;

            if (finalAmount <= 0)
            {
                throw new Exception("Số tiền thanh toán không hợp lệ");
            }

            string description = $"Order {orderId}";
            string returnUrl = $"https://localhost:7117/Order/OrderDetail?orderId={orderId}";
            string cancelUrl = $"https://localhost:7117/Order/OrderDetail?orderId={orderId}";

            string rawData =
                $"amount={finalAmount}" +
                $"&cancelUrl={cancelUrl}" +
                $"&description={description}" +
                $"&orderCode={orderCode}" +
                $"&returnUrl={returnUrl}";

            string signature = GenerateSignature(rawData, checksumKey);

            var body = new
            {
                orderCode,
                amount = finalAmount,
                description,
                cancelUrl,
                returnUrl,
                signature,
                items = new[]
                {
                    new
                    {
                        name = $"Order {orderId}",
                        quantity = 1,
                        price = finalAmount
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api-merchant.payos.vn/v2/payment-requests"
            );

            request.Headers.Add("x-client-id", clientId);
            request.Headers.Add("x-api-key", apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("=== PAYOS REQUEST ===");
            Console.WriteLine(json);
            Console.WriteLine("=== PAYOS RESPONSE ===");
            Console.WriteLine(result);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"PayOS lỗi: {result}");
            }

            using var doc = JsonDocument.Parse(result);

            if (!doc.RootElement.TryGetProperty("data", out var dataElement))
            {
                throw new Exception("PayOS không trả về data");
            }

            if (!dataElement.TryGetProperty("checkoutUrl", out var checkoutUrlElement))
            {
                throw new Exception("PayOS không trả về checkoutUrl");
            }

            var checkoutUrl = checkoutUrlElement.GetString();

            if (string.IsNullOrWhiteSpace(checkoutUrl))
            {
                throw new Exception("checkoutUrl rỗng");
            }

            return checkoutUrl;
        }

        private string GenerateSignature(string data, string checksumKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
using System.Net.Http.Headers;
using System.Text.Json;
using Intelligence_Book_WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_WEB.Controllers
{
    public class BookDetailController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookDetailController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var model = new BookDetailViewModel();

            // 1. Lấy thông tin sách
            var bookResponse = await client.GetAsync($"api/BookAPI/{id}");
            if (bookResponse.IsSuccessStatusCode)
            {
                var bookJson = await bookResponse.Content.ReadAsStringAsync();
                model.Book = JsonSerializer.Deserialize<BookViewModel>(bookJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new BookViewModel();
            }

            // 2. Lấy danh sách reviews
            var reviewsResponse = await client.GetAsync($"api/Reviews/book/{id}");
            if (reviewsResponse.IsSuccessStatusCode)
            {
                var reviewsJson = await reviewsResponse.Content.ReadAsStringAsync();
                model.Reviews = JsonSerializer.Deserialize<List<ReviewViewModel>>(reviewsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ReviewViewModel>();
            }

            // 3. Nếu user đã login, check eligibility và tìm review của họ
            var token = Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                // Check eligibility
                var eligibilityResponse = await client.GetAsync($"api/Reviews/check-eligibility/{id}");
                if (eligibilityResponse.IsSuccessStatusCode)
                {
                    var eligibilityJson = await eligibilityResponse.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(eligibilityJson);
                    model.CanReview = doc.RootElement.GetProperty("canReview").GetBoolean();
                }

                // Tìm UserReview trong list reviews (Dựa trên UserId trong token - ở đây giả định lấy từ Claims hoặc API trả về match)
                // Một cách an toàn hơn là API check-eligibility trả về luôn ReviewId nếu đã review.
                // Để đơn giản, ta tìm review có UserName khớp hoặc thêm link API lấy review của tôi.
                // Tạm thời giả định API check-eligibility trả về model chi tiết hơn.
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview([FromBody] ReviewFormDTO form)
        {
            var token = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(token)) return Unauthorized();

            var client = _httpClientFactory.CreateClient("MyAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            if (form.ReviewId.HasValue && form.ReviewId.Value > 0)
            {
                // Update
                response = await client.PutAsJsonAsync($"api/Reviews/{form.ReviewId.Value}", new { form.Rating, form.Comment });
            }
            else
            {
                // Create
                response = await client.PostAsJsonAsync("api/Reviews", new { form.BookId, form.Rating, form.Comment });
            }

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { success = true });
            }

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(new { success = false, message = error });
        }
    }
}

using System.Net.Http.Headers;
using System.Text.Json;
using Intelligence_Book_WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_WEB.Controllers
{
    [Route("Book")]
    public class BookController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BookController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var model = new BookDetailViewModel();

            // 1. Lấy thông tin sách
            var bookResponse = await client.GetAsync($"api/Books/{id}");
            if (bookResponse.IsSuccessStatusCode)
            {
                var bookJson = await bookResponse.Content.ReadAsStringAsync();
                model.Book = JsonSerializer.Deserialize<BookViewModel>(bookJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new BookViewModel();
            }
            else
            {
                return NotFound();
            }

            // 2. Lấy danh sách reviews
            var reviewsResponse = await client.GetAsync($"api/Reviews/book/{id}");
            if (reviewsResponse.IsSuccessStatusCode)
            {
                var reviewsJson = await reviewsResponse.Content.ReadAsStringAsync();
                model.Reviews = JsonSerializer.Deserialize<List<ReviewViewModel>>(reviewsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ReviewViewModel>();
            }

            // 3. Nếu user đã login, check eligibility
            var token = Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var eligibilityResponse = await client.GetAsync($"api/Reviews/check-eligibility/{id}");
                if (eligibilityResponse.IsSuccessStatusCode)
                {
                    var eligibilityJson = await eligibilityResponse.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(eligibilityJson);
                    model.CanReview = doc.RootElement.GetProperty("canReview").GetBoolean();
                }
            }

            return View(model);
        }

        [HttpPost("SubmitReview")]
        public async Task<IActionResult> SubmitReview([FromBody] ReviewFormDTO form)
        {
            var token = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(token)) return Unauthorized();

            var client = _httpClientFactory.CreateClient("MyAPI");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            if (form.ReviewId.HasValue && form.ReviewId.Value > 0)
            {
                response = await client.PutAsJsonAsync($"api/Reviews/{form.ReviewId.Value}", new { form.Rating, form.Comment });
            }
            else
            {
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



using System.Diagnostics;
using System.Text.Json;
using Intelligence_Book_WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_WEB.Controllers
{
    // Giúp class này nhận diện các URL bắt đầu bằng /Home hoặc / (trang chủ)
    [Route("Home")]
    [Route("")]
    public class HomeController_Anh : Controller
    {
        private readonly ILogger<HomeController_Anh> _logger;
        private readonly IHttpClientFactory _factory;

        public HomeController_Anh(ILogger<HomeController_Anh> logger, IHttpClientFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        [HttpGet("")] // Trang chủ mặc định: https://localhost:7117/
        [HttpGet("Index")] // https://localhost:7117/Home/Index
        public async Task<IActionResult> Index(string? search, decimal? minPrice, decimal? maxPrice, int? categoryId)
        {
            var client = _factory.CreateClient("MyAPI");
            var books = new List<BookViewModel>();
            bool isSearching = !string.IsNullOrEmpty(search) || minPrice.HasValue || maxPrice.HasValue || categoryId.HasValue;

            // 1. Luôn lấy Categories để hiển thị ở bộ lọc
            var cateResponse = await client.GetAsync("api/Categories");
            var categories = new List<CategoryViewModel>();
            if (cateResponse.IsSuccessStatusCode)
            {
                var cateJson = await cateResponse.Content.ReadAsStringAsync();
                categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(cateJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CategoryViewModel>();
            }

            // 2. Nếu có search params, lấy danh sách sách theo filter
            if (isSearching)
            {
                var url = $"api/Books?search={search}&categoryId={categoryId}&minPrice={minPrice}&maxPrice={maxPrice}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    books = JsonSerializer.Deserialize<List<BookViewModel>>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<BookViewModel>();
                }
            }

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.IsSearching = isSearching;
            ViewBag.SearchTerm = search;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View("~/Views/Home/Index.cshtml", books);
        }

        [HttpGet("Details/{id}")] // https://localhost:7117/Home/Details/5
        public async Task<IActionResult> Details(int id)
        {
            return RedirectToAction("Details", "Book", new { id });
        }


        }
    }

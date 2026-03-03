using System.Diagnostics;
using System.Text.Json;
using Intelligence_Book_WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_WEB.Controllers
{
    public class HomeController_Anh : Controller
    {
        private readonly ILogger<HomeController_Anh> _logger;
        private readonly IHttpClientFactory _factory;

        public HomeController_Anh(
            ILogger<HomeController_Anh> logger,
            IHttpClientFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        // =========================
        // TRANG CHỦ (WELCOME)  /
        // =========================
        public IActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }

        // =========================
        // TRANG SEARCH  /HomeController_Anh/Search
        // =========================
        public async Task<IActionResult> Search(
            string? search,
            decimal? minPrice,
            decimal? maxPrice,
            int? categoryId)
        {
            var client = _factory.CreateClient("BookAPI");

            var url =
                $"books?search={search}&categoryId={categoryId}&minPrice={minPrice}&maxPrice={maxPrice}";

            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            var books = JsonSerializer.Deserialize<List<BookViewModel>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var cateResponse = await client.GetAsync("categories");
            var cateJson = await cateResponse.Content.ReadAsStringAsync();

            var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(cateJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;

            return View("~/Views/Home/Search.cshtml", books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _factory.CreateClient("BookAPI");

            var response = await client.GetAsync($"books/{id}");
            var json = await response.Content.ReadAsStringAsync();

            var book = JsonSerializer.Deserialize<BookViewModel>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return View("~/Views/Home/BookDetails.cshtml", book);
        }
    }
}
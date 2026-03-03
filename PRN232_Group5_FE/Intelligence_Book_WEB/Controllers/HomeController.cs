using System.Diagnostics;
using System.Text.Json;
using Intelligence_Book_WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace Intelligence_Book_WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _factory;

        public HomeController(
            ILogger<HomeController> logger,
            IHttpClientFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        // HIỂN THỊ DANH SÁCH + SEARCH + FILTER
        public async Task<IActionResult> Index(
         string? search,
         decimal? minPrice,
         decimal? maxPrice,
         int? categoryId)
        {
            var client = _factory.CreateClient("BookAPI");

            // 🔹 Load books
            var url =
                $"books?search={search}&categoryId={categoryId}&minPrice={minPrice}&maxPrice={maxPrice}";

            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            var books = JsonSerializer.Deserialize<List<BookViewModel>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            // 🔹 Load categories
            var cateResponse = await client.GetAsync("categories");
            var cateJson = await cateResponse.Content.ReadAsStringAsync();

            var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(cateJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;

            return View(books);
        }

        // XEM CHI TIẾT
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

            return View("BookDetails", book);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ??
                            HttpContext.TraceIdentifier
            });
        }
    }

}
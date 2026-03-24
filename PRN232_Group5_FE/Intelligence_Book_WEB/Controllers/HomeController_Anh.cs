//using System.Diagnostics;
//using System.Text.Json;
//using Intelligence_Book_WEB.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace Intelligence_Book_WEB.Controllers
//{

//    public class HomeController_Anh : Controller
//    {
//        private readonly ILogger<HomeController_Anh> _logger;
//        private readonly IHttpClientFactory _factory;

//        public HomeController_Anh(
//            ILogger<HomeController_Anh> logger,
//            IHttpClientFactory factory)
//        {
//            _logger = logger;
//            _factory = factory;
//        }

//        // =========================
//        // TRANG CHỦ (WELCOME)  /
//        // =========================
//        public IActionResult Index()
//        {
//            return View("~/Views/Home/Index.cshtml");
//        }

//        // =========================
//        // TRANG SEARCH  /HomeController_Anh/Search
//        // =========================
//        public async Task<IActionResult> Search(
//            string? search,
//            decimal? minPrice,
//            decimal? maxPrice,
//            int? categoryId)
//        {
//            var client = _factory.CreateClient("BookAPI");

//            var url =
//                $"books?search={search}&categoryId={categoryId}&minPrice={minPrice}&maxPrice={maxPrice}";

//            var response = await client.GetAsync(url);
//            var json = await response.Content.ReadAsStringAsync();

//            var books = JsonSerializer.Deserialize<List<BookViewModel>>(json,
//                new JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                });

//            var cateResponse = await client.GetAsync("categories");
//            var cateJson = await cateResponse.Content.ReadAsStringAsync();

//            var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(cateJson,
//                new JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                });

//            ViewBag.Categories = categories;
//            ViewBag.SelectedCategory = categoryId;

//            return View("~/Views/Home/Search.cshtml", books);
//        }

//        public async Task<IActionResult> Details(int id)
//        {
//            var client = _factory.CreateClient("BookAPI");

//            var response = await client.GetAsync($"books/{id}");
//            var json = await response.Content.ReadAsStringAsync();

//            var book = JsonSerializer.Deserialize<BookViewModel>(json,
//                new JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                });

//            return View("~/Views/Home/BookDetails.cshtml", book);
//        }
//        // Trả view Wishlist
//        // Trả view Wishlist
//        public async Task<IActionResult> Wishlist()
//        {
//            var client = _factory.CreateClient("BookAPI");

//            var wishlistIdsQuery = Request.Query["ids"].ToString();
//            var wishlistIds = wishlistIdsQuery.Split(',', StringSplitOptions.RemoveEmptyEntries)
//                                              .Select(int.Parse)
//                                              .ToList();

//            var books = new List<BookViewModel>();

//            foreach (var id in wishlistIds)
//            {
//                var response = await client.GetAsync($"books/{id}");
//                if (response.IsSuccessStatusCode)
//                {
//                    var json = await response.Content.ReadAsStringAsync();
//                    var book = JsonSerializer.Deserialize<BookViewModel>(json,
//                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
//                    if (book != null) books.Add(book);
//                }
//            }

//            return View("~/Views/Home/Wishlist.cshtml", books);
//        }
//    }
//}

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
        public IActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpGet("Search")] // https://localhost:7117/Home/Search
        public async Task<IActionResult> Search(string? search, decimal? minPrice, decimal? maxPrice, int? categoryId)
        {
            var client = _factory.CreateClient("BookAPI");
            var url = $"books?search={search}&categoryId={categoryId}&minPrice={minPrice}&maxPrice={maxPrice}";

            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            var books = JsonSerializer.Deserialize<List<BookViewModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var cateResponse = await client.GetAsync("categories");
            var cateJson = await cateResponse.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(cateJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;

            return View("~/Views/User/Search.cshtml", books);
        }

        [HttpGet("Details/{id}")] // https://localhost:7117/Home/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = _factory.CreateClient("BookAPI");
            var response = await client.GetAsync($"books/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var book = JsonSerializer.Deserialize<BookViewModel>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View("~/Views/User/BookDetails.cshtml", book);
        }

        [HttpGet("/User/Wishlist")] // Giữ nguyên link ngắn của bạn
        public async Task<IActionResult> Wishlist()
        {
            var client = _factory.CreateClient("BookAPI");
            var wishlistIdsQuery = Request.Query["ids"].ToString();

            if (string.IsNullOrEmpty(wishlistIdsQuery))
                return View("~/Views/User/Wishlist.cshtml", new List<BookViewModel>());

            var wishlistIds = wishlistIdsQuery.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                             .Select(int.Parse)
                                             .ToList();

            var books = new List<BookViewModel>();
            foreach (var id in wishlistIds)
            {
                var response = await client.GetAsync($"books/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var book = JsonSerializer.Deserialize<BookViewModel>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (book != null) books.Add(book);
                }
            }
            return View("~/Views/User/Wishlist.cshtml", books);
        }
    }
}
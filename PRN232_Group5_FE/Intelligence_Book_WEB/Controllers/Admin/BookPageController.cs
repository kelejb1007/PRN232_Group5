using Intelligence_Book_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Intelligence_Book_WEB.Controllers.Admin
{
    public class BookPageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BookPageController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.GetAsync("api/Admin/categories");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CategoryViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CategoryViewModel>();
            }
            return new List<CategoryViewModel>();
        }

        public async Task<IActionResult> Index(string? search, List<int>? categoryIds)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            // Build query string
            var query = new List<string>();
            if (!string.IsNullOrWhiteSpace(search))
                query.Add($"search={Uri.EscapeDataString(search)}");
            if (categoryIds != null && categoryIds.Any())
                foreach (var id in categoryIds)
                    query.Add($"categoryIds={id}");

            var url = query.Any() ? $"api/Admin/Books?{string.Join("&", query)}" : "api/Admin/Books";
            var response = await client.GetAsync(url);

            var books = new List<BookViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                books = JsonSerializer.Deserialize<List<BookViewModel>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? books;
            }

            // Pass categories and current filter state to view
            ViewBag.Categories = await GetCategoriesAsync();
            ViewBag.Search = search;
            ViewBag.SelectedCategories = categoryIds ?? new List<int>();

            return View(books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.GetAsync($"api/Admin/Books/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var book = JsonSerializer.Deserialize<BookViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(book);
        }

        public async Task<IActionResult> Create()
        {
            var model = new BookCreateViewModel
            {
                AvailableCategories = await GetCategoriesAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableCategories = await GetCategoriesAsync();
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("MyAPI");
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Title ?? ""), "Title");
            content.Add(new StringContent(model.Price.ToString()), "Price");
            if (!string.IsNullOrEmpty(model.ISBN)) content.Add(new StringContent(model.ISBN), "ISBN");
            if (!string.IsNullOrEmpty(model.AuthorName)) content.Add(new StringContent(model.AuthorName), "AuthorName");
            if (!string.IsNullOrEmpty(model.Publisher)) content.Add(new StringContent(model.Publisher), "Publisher");
            if (model.PublishDate.HasValue) content.Add(new StringContent(model.PublishDate.Value.ToString("yyyy-MM-dd")), "PublishDate");
            if (!string.IsNullOrEmpty(model.Description)) content.Add(new StringContent(model.Description), "Description");
            content.Add(new StringContent(model.StockQuantity.ToString()), "StockQuantity");

            if (model.CoverImageUrl != null)
            {
                var streamContent = new StreamContent(model.CoverImageUrl.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.CoverImageUrl.ContentType);
                content.Add(streamContent, "CoverImageUrl", model.CoverImageUrl.FileName);
            }

            if (model.CategoryIds != null)
            {
                foreach (var catId in model.CategoryIds)
                {
                    content.Add(new StringContent(catId.ToString()), "CategoryIds");
                }
            }

            var response = await client.PostAsync("api/Admin/Books", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Book created successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to create book.";
            ModelState.AddModelError("", "Failed to create book.");
            model.AvailableCategories = await GetCategoriesAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.GetAsync($"api/Admin/Books/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var book = JsonSerializer.Deserialize<BookViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (book == null) return NotFound();

            var model = new BookUpdateViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Price = book.Price,
                ISBN = book.ISBN,
                AuthorName = book.AuthorName,
                Publisher = book.Publisher,
                PublishDate = book.PublishDate,
                ExistingCoverImageUrl = book.CoverImageUrl,
                Description = book.Description,
                StockQuantity = book.StockQuantity,
                CategoryIds = book.Categories?.Select(c => c.CategoryId).ToList() ?? new List<int>(),
                AvailableCategories = await GetCategoriesAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableCategories = await GetCategoriesAsync();
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("MyAPI");
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Title ?? ""), "Title");
            content.Add(new StringContent(model.Price.ToString()), "Price");
            if (!string.IsNullOrEmpty(model.ISBN)) content.Add(new StringContent(model.ISBN), "ISBN");
            if (!string.IsNullOrEmpty(model.AuthorName)) content.Add(new StringContent(model.AuthorName), "AuthorName");
            if (!string.IsNullOrEmpty(model.Publisher)) content.Add(new StringContent(model.Publisher), "Publisher");
            if (model.PublishDate.HasValue) content.Add(new StringContent(model.PublishDate.Value.ToString("yyyy-MM-dd")), "PublishDate");
            if (!string.IsNullOrEmpty(model.Description)) content.Add(new StringContent(model.Description), "Description");
            content.Add(new StringContent(model.StockQuantity.ToString()), "StockQuantity");

            if (model.CoverImageUrl != null)
            {
                var streamContent = new StreamContent(model.CoverImageUrl.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.CoverImageUrl.ContentType);
                content.Add(streamContent, "CoverImageUrl", model.CoverImageUrl.FileName);
            }

            if (model.CategoryIds != null)
            {
                foreach (var catId in model.CategoryIds)
                {
                    content.Add(new StringContent(catId.ToString()), "CategoryIds");
                }
            }

            var response = await client.PutAsync($"api/Admin/Books/{model.BookId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Book updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to update book.";
            ModelState.AddModelError("", "Failed to update book.");
            model.AvailableCategories = await GetCategoriesAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.DeleteAsync($"api/Admin/Books/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Book deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Delete failed!";
            }

            return RedirectToAction("Index");
        }
    }
}

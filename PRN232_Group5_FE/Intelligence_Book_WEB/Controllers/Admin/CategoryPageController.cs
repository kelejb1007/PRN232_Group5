using Intelligence_Book_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Intelligence_Book_WEB.Controllers.Admin
{
    public class CategoryPageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoryPageController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            var url = string.IsNullOrWhiteSpace(search)
                ? "api/Admin/categories"
                : $"api/Admin/categories?search={Uri.EscapeDataString(search)}";

            var response = await client.GetAsync(url);

            var categories = new List<CategoryViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? categories;
            }

            ViewBag.Search = search;
            ViewBag.HasFilter = !string.IsNullOrWhiteSpace(search);

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            var response = await client.GetAsync($"api/Admin/categories/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();

            var category = JsonSerializer.Deserialize<CategoryViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("MyAPI");

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(model),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("api/Admin/categories", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to create category.";
            ModelState.AddModelError("", "Failed to create category.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("MyAPI");

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(model),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await client.PutAsync(
                $"api/Admin/categories/{model.CategoryId}",
                jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Update failed.";
            ModelState.AddModelError("", "Update failed.");
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            var response = await client.DeleteAsync($"api/Admin/categories/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Delete failed!";
            }

            return RedirectToAction("Index");
        }
    }
}

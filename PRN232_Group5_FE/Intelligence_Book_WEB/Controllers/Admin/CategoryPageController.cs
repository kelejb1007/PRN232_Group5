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

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            var response = await client.GetAsync("api/categories");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<CategoryViewModel>());
            }

            var json = await response.Content.ReadAsStringAsync();

            var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            var response = await client.GetAsync($"api/categories/{id}");

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

            var response = await client.PostAsync("api/categories", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction("Index");
            }

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
                $"api/categories/{model.CategoryId}",
                jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Update failed.");
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            var response = await client.DeleteAsync($"api/categories/{id}");

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

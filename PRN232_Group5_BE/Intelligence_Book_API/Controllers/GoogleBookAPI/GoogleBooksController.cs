using System.Text.Json;
using DAL.DTOs.BookDTO;
using Microsoft.AspNetCore.Mvc;
using static DAL.DTOs.BookDTO.GoogleBookAPI;

namespace Intelligence_Book_API.Controllers.GoogleBookAPI
{
    [ApiController] // Cần có attribute này cho API Controller
    [Route("api/[controller]")]
    public class GoogleBooksController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        // Tiêm (Inject) HttpClient vào controller
        public GoogleBooksController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBookAsync([FromQuery] string bookName)
        {
            if (string.IsNullOrWhiteSpace(bookName))
            {
                return BadRequest("Tên sách không được để trống.");
            }

            // 1. Tạo URL gọi Google Books API
            string url = $"https://www.googleapis.com/books/v1/volumes?q={Uri.EscapeDataString(bookName)}";

            try
            {
                // 2. Gọi HTTP GET
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // 3. Đọc và Deserialize JSON
                string jsonString = await response.Content.ReadAsStringAsync();
                var googleData = JsonSerializer.Deserialize<GoogleBooksResponse>(jsonString);

                // Nếu không tìm thấy sách
                if (googleData?.Items == null || !googleData.Items.Any())
                {
                    return NotFound("Không tìm thấy dữ liệu từ Google Books.");
                }

                // 4. Bóc tách, Lọc và map sang DTO của bạn
                var resultList = new List<GoogleBookAPIDTO>();

                foreach (var item in googleData.Items)
                {
                    var sale = item.SaleInfo;

                    // ==========================================
                    // BỘ LỌC TÌM SÁCH BÁN BẰNG VNĐ
                    // ==========================================
                    if (sale == null ||
                        sale.Saleability != "FOR_SALE" ||
                        sale.ListPrice == null ||
                        sale.ListPrice.CurrencyCode != "VND")
                    {
                        continue; // Bỏ qua cuốn sách này nếu không bán hoặc không phải VNĐ
                    }

                    var info = item.VolumeInfo;

                    // Lọc tìm ISBN-10 và ISBN-13
                    var isbn10 = info.IndustryIdentifiers?.FirstOrDefault(i => i.Type == "ISBN_10")?.Identifier;
                    var isbn13 = info.IndustryIdentifiers?.FirstOrDefault(i => i.Type == "ISBN_13")?.Identifier;

                    // Thêm vào danh sách kết quả
                    resultList.Add(new GoogleBookAPIDTO
                    {
                        Title = info.Title ?? "Không rõ",
                        Authors = info.Authors ?? new List<string>(),
                        CoverImage = info.ImageLinks?.Thumbnail ?? "",
                        Isbn10 = isbn10 ?? "",
                        Isbn13 = isbn13 ?? "",

                        // CÁC TRƯỜNG THÊM MỚI
                        Publisher = info.Publisher ?? "Không rõ",
                        PublishedDate = info.PublishedDate ?? "Không rõ",
                        Description = info.Description ?? "Chưa có mô tả",
                        Price = sale.ListPrice.Amount,
                        Currency = sale.ListPrice.CurrencyCode
                    });
                }

                // Kiểm tra xem sau khi lọc có còn cuốn nào không
                if (!resultList.Any())
                {
                    return NotFound("Không tìm thấy cuốn sách nào đang được bán bằng VNĐ.");
                }

                // 5. Trả kết quả về cho Frontend, tự động sắp xếp giá từ thấp đến cao
                return Ok(resultList.OrderBy(b => b.Price).ToList());
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Lỗi khi gọi Google API: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return StatusCode(500, $"Lỗi khi đọc dữ liệu từ Google: {ex.Message}");
            }
        }
    }
}

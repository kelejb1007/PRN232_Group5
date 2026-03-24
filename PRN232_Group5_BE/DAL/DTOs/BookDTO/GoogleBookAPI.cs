using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.DTOs.BookDTO
{
    public class GoogleBookAPI
    {
        public class GoogleBooksResponse
        {
            [JsonPropertyName("items")]
            public List<BookItem> Items { get; set; }
        }

        public class BookItem
        {
            [JsonPropertyName("volumeInfo")]
            public VolumeInfo VolumeInfo { get; set; }

            // [THÊM MỚI] Chứa thông tin về giá bán và trạng thái bán
            [JsonPropertyName("saleInfo")]
            public SaleInfo SaleInfo { get; set; }
        }

        public class VolumeInfo
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("authors")]
            public List<string> Authors { get; set; }

            // [THÊM MỚI] Nhà xuất bản
            [JsonPropertyName("publisher")]
            public string Publisher { get; set; }

            // [THÊM MỚI] Ngày xuất bản (Lưu ý: Dùng string vì Google có thể trả về "2020" hoặc "2020-12-05")
            [JsonPropertyName("publishedDate")]
            public string PublishedDate { get; set; }

            // [THÊM MỚI] Mô tả nội dung sách
            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("industryIdentifiers")]
            public List<IndustryIdentifier> IndustryIdentifiers { get; set; }

            [JsonPropertyName("imageLinks")]
            public ImageLinks ImageLinks { get; set; }
        }

        public class IndustryIdentifier
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("identifier")]
            public string Identifier { get; set; }
        }

        public class ImageLinks
        {
            [JsonPropertyName("thumbnail")]
            public string Thumbnail { get; set; }
        }

        // ==========================================
        // CÁC CLASS MỚI ĐỂ XỬ LÝ GIÁ TIỀN (SALE INFO)
        // ==========================================
        public class SaleInfo
        {
            [JsonPropertyName("saleability")]
            public string Saleability { get; set; } // Trả về "FOR_SALE", "NOT_FOR_SALE", hoặc "FREE"

            [JsonPropertyName("listPrice")]
            public PriceInfo ListPrice { get; set; }
        }

        public class PriceInfo
        {
            [JsonPropertyName("amount")]
            public double Amount { get; set; }

            [JsonPropertyName("currencyCode")]
            public string CurrencyCode { get; set; } // VD: "VND", "USD"
        }
    }
}

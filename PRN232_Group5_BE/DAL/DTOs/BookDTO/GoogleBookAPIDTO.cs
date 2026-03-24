using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTOs.BookDTO
{
    public class GoogleBookAPIDTO
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string CoverImage { get; set; }
        public string Isbn10 { get; set; }
        public string Isbn13 { get; set; }

        // CÁC TRƯỜNG MỚI
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        public string Description { get; set; }

        // Giá gộp chung lại cho gọn (VD: 150000 hoặc 0 nếu không bán)
        public double Price { get; set; }
        public string Currency { get; set; }
    }
}

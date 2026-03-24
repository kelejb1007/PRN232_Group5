using System;

namespace Intelligence_Book_WEB.Models.Dto
{
    public class OrderItemVm
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public string? BookName { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public decimal TotalPrice => Quantity * PriceAtPurchase;
    }
}

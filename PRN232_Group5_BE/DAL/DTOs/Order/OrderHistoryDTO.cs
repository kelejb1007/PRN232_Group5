namespace DAL.DTOs.Order
{
    public class OrderHistoryDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ShippingAddress { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; } = new();
    }

    public class OrderItemDTO
    {
        public int OrderItemId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? BookImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

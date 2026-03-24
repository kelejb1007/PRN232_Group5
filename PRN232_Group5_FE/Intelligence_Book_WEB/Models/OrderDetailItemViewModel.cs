namespace Intelligence_Book_WEB.Models
{
    public class OrderDetailItemViewModel
    {
        public int OrderItemId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = "";
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public decimal SubTotal { get; set; }
    }
}

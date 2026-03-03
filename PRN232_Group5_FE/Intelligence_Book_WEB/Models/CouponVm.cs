namespace Intelligence_Book_WEB.Models
{
    public class CouponVm
    {
        public int CouponId { get; set; }
        public string Code { get; set; } = string.Empty;
        public int DiscountPercent { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int quantity { get; set; }  
    }
}

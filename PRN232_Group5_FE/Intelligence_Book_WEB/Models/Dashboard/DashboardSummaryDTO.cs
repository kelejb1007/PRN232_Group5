namespace Intelligence_Book_WEB.Models.Dashboard
{
    public class DashboardSummaryDTO
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalBooks { get; set; }
        public List<RevenueDataDTO> RevenueByMonth { get; set; } = new();
        public List<RevenueDataDTO> RevenueByDay { get; set; } = new();
        public List<TopProductDTO> TopSellingBooks { get; set; } = new();
        public List<LowStockProductDTO> LowStockBooks { get; set; } = new();
        public List<RegistrationDataDTO> RegistrationsByDay { get; set; } = new();
    }

    public class RevenueDataDTO
    {
        public string Label { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class TopProductDTO
    {
        public string BookTitle { get; set; } = string.Empty;
        public int UnitsSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class LowStockProductDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }

    public class RegistrationDataDTO
    {
        public string Date { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}

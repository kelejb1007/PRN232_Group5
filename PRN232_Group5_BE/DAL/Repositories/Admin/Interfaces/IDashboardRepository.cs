using DAL.DTOs.Dashboard;

namespace DAL.Repositories.Admin.Interfaces
{
    public interface IDashboardRepository
    {
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalOrdersAsync();
        Task<int> GetTotalCustomersAsync();
        Task<int> GetTotalBooksAsync();
        Task<List<RevenueDataDTO>> GetRevenueByMonthAsync(int year);
        Task<List<RevenueDataDTO>> GetRevenueByDayAsync(DateTime startDate, DateTime endDate);
        Task<List<TopProductDTO>> GetTopSellingBooksAsync(int count);
        Task<List<LowStockProductDTO>> GetLowStockBooksAsync(int threshold);
        Task<List<RegistrationDataDTO>> GetRegistrationsByDayAsync(int days);
    }
}
